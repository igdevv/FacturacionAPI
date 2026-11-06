using FacturacionAPI.Application.Facturas;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Enums;
using FacturacionAPI.Domain.Excepciones;
using FacturacionAPI.Infrastructure.Persistence;
using FacturacionAPI.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FacturacionAPI.Domain.Tests;

// Se omiten explícitamente si no se proporciona una base local de pruebas ya migrada.
public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FACTURACION_TEST_SQLSERVER")))
            Skip = "Configura FACTURACION_TEST_SQLSERVER con la base local FacturacionAPI_V1_Pruebas.";
    }
}

public class PersistenciaSqlServerTests
{
    [SqlServerFact]
    public async Task CrearConsultarEmitir_PersisteGrafoYPrecioHistorico()
    {
        await using var sesion = await SesionSql.CrearAsync();
        var db = sesion.Db;
        var (cliente, producto) = await SembrarAsync(db);
        var repositorio = new FacturaRepository(db);
        var servicio = new CrearFacturaService(repositorio, new ClienteRepository(db), new ProductoRepository(db));
        var respuesta = await servicio.EjecutarAsync(new CrearFacturaDto
        {
            ClienteId = cliente.Id, Productos = [new() { ProductoId = producto.Id, Cantidad = 2 }]
        });
        db.ChangeTracker.Clear();
        var factura = await repositorio.ObtenerPorIdAsync(respuesta.Id);
        Assert.NotNull(factura);
        Assert.Equal(EstadoFactura.Borrador, factura.Estado);
        Assert.Equal(cliente.Id, factura.Cliente.Id);
        Assert.Equal(respuesta.Id, Assert.Single(factura.Detalles).FacturaId);
        Assert.Equal(236m, factura.Total);
        Assert.Equal(DateTimeKind.Utc, factura.FechaEmision.Kind);
        Assert.Empty(db.ChangeTracker.Entries());
        Assert.Contains(await repositorio.ObtenerTodosAsync(), f => f.Id == respuesta.Id);

        var productoActual = await db.Productos.SingleAsync(p => p.Id == producto.Id);
        productoActual.CambiarPrecio(200m);
        await db.SaveChangesAsync();
        await new EmitirFacturaService(repositorio).EjecutarAsync(respuesta.Id);
        db.ChangeTracker.Clear();
        var emitida = await repositorio.ObtenerPorIdAsync(respuesta.Id);
        Assert.Equal(EstadoFactura.Emitida, emitida!.Estado);
        Assert.Equal(100m, emitida.Detalles.Single().PrecioUnitario);
        Assert.Equal(236m, emitida.Total);
        Assert.Equal(10, emitida.Detalles.Single().Producto.Stock);
        await Assert.ThrowsAsync<ConflictoNegocioException>(() =>
            new EmitirFacturaService(repositorio).EjecutarAsync(respuesta.Id));
    }

    [SqlServerFact]
    public async Task Stock_ActualizacionesSimultaneasNoPierdenCambios()
    {
        await using var sesion = await SesionSql.CrearAsync();
        var (_, producto) = await SembrarAsync(sesion.Db);
        await using var otro = await sesion.OtroContextoAsync();
        var repositorio1 = new ProductoRepository(sesion.Db);
        var repositorio2 = new ProductoRepository(otro);
        var primero = (await repositorio1.ObtenerPorIdAsync(producto.Id, true))!;
        var segundo = (await repositorio2.ObtenerPorIdAsync(producto.Id, true))!;
        primero.DescontarStock(7);
        segundo.DescontarStock(7);
        await repositorio1.ActualizarAsync(primero);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repositorio2.ActualizarAsync(segundo));
        sesion.Db.ChangeTracker.Clear();
        Assert.Equal(3, (await sesion.Db.Productos.FindAsync(producto.Id))!.Stock);
    }

    [SqlServerFact]
    public async Task Emitir_ConflictoSiOtraSolicitudYaEmitio()
    {
        await using var sesion = await SesionSql.CrearAsync();
        var (cliente, producto) = await SembrarAsync(sesion.Db);
        var factura = new Factura(cliente.Id);
        factura.AgregarDetalle(new DetalleFactura(producto.Id, 1, producto.Precio, .18m));
        var repo1 = new FacturaRepository(sesion.Db);
        await repo1.AgregarAsync(factura);
        await using var otro = await sesion.OtroContextoAsync();
        var repo2 = new FacturaRepository(otro);
        var primero = (await repo1.ObtenerPorIdAsync(factura.Id, true))!;
        var segundo = (await repo2.ObtenerPorIdAsync(factura.Id, true))!;
        primero.EmitirFactura();
        segundo.EmitirFactura();
        await repo1.GuardarCambiosAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repo2.GuardarCambiosAsync());
    }

    [SqlServerFact]
    public async Task Crear_DetalleConProductoInexistenteRevierteCabecera()
    {
        await using var sesion = await SesionSql.CrearAsync();
        var (cliente, producto) = await SembrarAsync(sesion.Db);
        var cantidadAntes = await sesion.Db.Facturas.CountAsync();
        var factura = new Factura(cliente.Id);
        factura.AgregarDetalle(new DetalleFactura(producto.Id, 1, 10m, .18m));
        factura.AgregarDetalle(new DetalleFactura(int.MaxValue, 1, 10m, .18m));
        await Assert.ThrowsAsync<DbUpdateException>(() => new FacturaRepository(sesion.Db).AgregarAsync(factura));
        sesion.Db.ChangeTracker.Clear();
        Assert.Equal(cantidadAntes, await sesion.Db.Facturas.CountAsync());
    }

    [SqlServerFact]
    public async Task Cliente_DocumentoDuplicadoSeTraduceAConflicto()
    {
        await using var sesion = await SesionSql.CrearAsync();
        var (cliente, _) = await SembrarAsync(sesion.Db);
        await Assert.ThrowsAsync<ConflictoNegocioException>(() => new ClienteRepository(sesion.Db)
            .AgregarAsync(new Cliente("Duplicado", cliente.RncCedula, null, null)));
    }

    [Fact]
    public void Modelo_CoincideConMigracionesYConfiguraRelaciones()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FacturacionAPI_V1_Pruebas;Integrated Security=true").Options);
        Assert.False(db.Database.HasPendingModelChanges());
        var factura = db.Model.FindEntityType(typeof(Factura))!;
        Assert.Equal("_detalles", factura.FindNavigation(nameof(Factura.Detalles))!.FieldInfo!.Name);
        Assert.True(factura.FindProperty(nameof(Factura.Estado))!.IsConcurrencyToken);
        Assert.Single(factura.GetForeignKeys());
        Assert.Equal(2, db.Model.FindEntityType(typeof(DetalleFactura))!.GetForeignKeys().Count());
    }

    private static async Task<(Cliente, Producto)> SembrarAsync(AppDbContext db)
    {
        var cliente = new Cliente("Prueba V1", Guid.NewGuid().ToString("N"), null, null);
        var producto = new Producto("Producto prueba V1", 100m, 10);
        db.AddRange(cliente, producto);
        await db.SaveChangesAsync();
        return (cliente, producto);
    }

    private sealed class SesionSql(AppDbContext db, IDbContextTransaction transaccion) : IAsyncDisposable
    {
        public AppDbContext Db => db;
        public static async Task<SesionSql> CrearAsync()
        {
            var conexion = Environment.GetEnvironmentVariable("FACTURACION_TEST_SQLSERVER")!;
            var cs = new SqlConnectionStringBuilder(conexion);
            if (cs.InitialCatalog != "FacturacionAPI_V1_Pruebas" ||
                !cs.DataSource.Equals(@"(localdb)\MSSQLLocalDB", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Las pruebas solo admiten LocalDB y FacturacionAPI_V1_Pruebas.");
            var contexto = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(conexion).Options);
            return new SesionSql(contexto, await contexto.Database.BeginTransactionAsync());
        }

        public async Task<AppDbContext> OtroContextoAsync()
        {
            var otro = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(db.Database.GetDbConnection()).Options);
            await otro.Database.UseTransactionAsync(transaccion.GetDbTransaction());
            return otro;
        }

        public async ValueTask DisposeAsync()
        {
            await transaccion.RollbackAsync();
            await transaccion.DisposeAsync();
            await db.DisposeAsync();
        }
    }
}
