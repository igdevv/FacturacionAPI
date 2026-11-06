using FacturacionAPI.Application.Excepciones;
using FacturacionAPI.Application.Facturas;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Enums;

namespace FacturacionAPI.Domain.Tests;

public class CrearFacturaServiceTests
{
    [Fact]
    public async Task Crear_UsaPreciosRealesYGuardaBorradorCompleto()
    {
        var repos = new RepositoriosPrueba();
        repos.Productos.Add(ConId(new Producto("Mouse", 100m, 0), 1));
        repos.Productos.Add(ConId(new Producto("Teclado", 50m, 10), 2));
        var dto = Peticion();
        dto.Productos.Add(new() { ProductoId = 2, Cantidad = 1 });

        var respuesta = await new CrearFacturaService(repos, repos, repos).EjecutarAsync(dto);

        Assert.Equal("Borrador", respuesta.Estado);
        Assert.Equal(250m, respuesta.Subtotal);
        Assert.Equal(45m, respuesta.Itbis);
        Assert.Equal(295m, respuesta.Total);
        Assert.Equal(2, respuesta.Detalles.Count);
        Assert.Equal(100m, respuesta.Detalles[0].PrecioUnitario);
        Assert.Equal(0, repos.Productos[0].Stock);
        Assert.Equal(1, repos.ConsultasProductos);
        Assert.Equal(2, repos.FacturaGuardada!.Detalles.Count);
    }

    [Fact]
    public async Task Crear_ProductoInexistenteNoGuarda()
    {
        var repos = new RepositoriosPrueba();
        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() =>
            new CrearFacturaService(repos, repos, repos).EjecutarAsync(Peticion()));
        Assert.Null(repos.FacturaGuardada);
    }

    [Fact]
    public async Task Crear_ClienteInexistenteNoConsultaProductosNiGuarda()
    {
        var repos = new RepositoriosPrueba { Cliente = null };
        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() =>
            new CrearFacturaService(repos, repos, repos).EjecutarAsync(Peticion()));
        Assert.Equal(0, repos.ConsultasProductos);
        Assert.Null(repos.FacturaGuardada);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Crear_CantidadInvalidaNoGuarda(int cantidad)
    {
        var repos = new RepositoriosPrueba();
        var dto = Peticion();
        dto.Productos[0].Cantidad = cantidad;
        await Assert.ThrowsAsync<ArgumentException>(() =>
            new CrearFacturaService(repos, repos, repos).EjecutarAsync(dto));
        Assert.Null(repos.FacturaGuardada);
        Assert.Equal(0, repos.ConsultasProductos);
    }

    [Theory]
    [InlineData("vacia")]
    [InlineData("null")]
    [InlineData("lineaNull")]
    [InlineData("duplicado")]
    [InlineData("productoInvalido")]
    [InlineData("clienteInvalido")]
    [InlineData("desbordamiento")]
    public async Task Crear_EstructuraInvalidaNoGuarda(string caso)
    {
        var repos = new RepositoriosPrueba();
        var dto = Peticion();
        switch (caso)
        {
            case "vacia": dto.Productos.Clear(); break;
            case "null": dto.Productos = null!; break;
            case "lineaNull": dto.Productos.Add(null!); break;
            case "duplicado": dto.Productos.Add(dto.Productos[0]); break;
            case "productoInvalido": dto.Productos[0].ProductoId = 0; break;
            case "clienteInvalido": dto.ClienteId = 0; break;
            case "desbordamiento":
                dto.Productos[0].Cantidad = int.MaxValue;
                dto.Productos.Add(new() { ProductoId = 2, Cantidad = 1 });
                break;
        }
        await Assert.ThrowsAsync<ArgumentException>(() =>
            new CrearFacturaService(repos, repos, repos).EjecutarAsync(dto));
        Assert.Null(repos.FacturaGuardada);
    }

    [Fact]
    public async Task Emitir_UsaDominioYPersiste()
    {
        var repos = new RepositoriosPrueba();
        repos.FacturaGuardada = new Factura(1);
        repos.FacturaGuardada.AgregarDetalle(new DetalleFactura(1, 1, 100m, .18m));
        await new EmitirFacturaService(repos).EjecutarAsync(1);
        Assert.Equal(EstadoFactura.Emitida, repos.FacturaGuardada.Estado);
        Assert.Equal(1, repos.Guardados);
    }

    [Fact]
    public async Task Emitir_InexistenteDevuelveNoEncontrado()
    {
        var repos = new RepositoriosPrueba();
        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() => new EmitirFacturaService(repos).EjecutarAsync(1));
        Assert.Equal(0, repos.Guardados);
    }

    private static CrearFacturaDto Peticion() => new()
    {
        ClienteId = 1, Productos = [new() { ProductoId = 1, Cantidad = 2 }]
    };

    private static T ConId<T>(T entidad, int id)
    {
        typeof(T).GetProperty("Id")!.SetValue(entidad, id);
        return entidad;
    }

    // Dobles mínimos sin bibliotecas de mocking; las pruebas SQL usan los repositorios reales.
    private sealed class RepositoriosPrueba : IFacturaRepository, IClienteRepository, IProductoRepository
    {
        public Cliente? Cliente { get; set; } = ConId(new Cliente("Ana", "001", null, null), 1);
        public List<Producto> Productos { get; } = [];
        public Factura? FacturaGuardada { get; set; }
        public int ConsultasProductos { get; private set; }
        public int Guardados { get; private set; }
        public Task AgregarAsync(Factura factura) { FacturaGuardada = ConId(factura, 1); return Task.CompletedTask; }
        public Task<Factura?> ObtenerPorIdAsync(int id, bool seguimiento = false) => Task.FromResult(FacturaGuardada);
        Task<List<Factura>> IFacturaRepository.ObtenerTodosAsync() => Task.FromResult(new List<Factura>());
        public Task GuardarCambiosAsync() { Guardados++; return Task.CompletedTask; }
        Task<Cliente?> IClienteRepository.ObtenerPorIdAsync(int id) => Task.FromResult(Cliente);
        public Task<bool> ExistePorDocumentoAsync(string documento) => Task.FromResult(false);
        public Task AgregarAsync(Cliente cliente) => Task.CompletedTask;
        Task<List<Cliente>> IClienteRepository.ObtenerTodosAsync() => Task.FromResult(new List<Cliente>());
        public Task AgregarAsync(Producto producto) => Task.CompletedTask;
        Task<List<Producto>> IProductoRepository.ObtenerTodosAsync() => Task.FromResult(Productos);
        Task<Producto?> IProductoRepository.ObtenerPorIdAsync(int id, bool seguimiento) => Task.FromResult(Productos.Find(p => p.Id == id));
        public Task ActualizarAsync(Producto producto) => Task.CompletedTask;
        public Task<List<Producto>> BuscarPorNombreAsync(string nombre) => Task.FromResult(Productos);
        public Task<List<Producto>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids)
        {
            ConsultasProductos++;
            return Task.FromResult(Productos.Where(p => ids.Contains(p.Id)).ToList());
        }
    }
}
