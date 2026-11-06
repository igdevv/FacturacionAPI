using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Enums;
using FacturacionAPI.Domain.Excepciones;

namespace FacturacionAPI.Domain.Tests;

public class FacturaTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Crear_ClienteInvalido(int id) =>
        Assert.Throws<ArgumentException>(() => new Factura(id));

    [Fact]
    public void NuevaFactura_EsBorradorSinTotales()
    {
        var factura = new Factura(1);
        Assert.Equal(EstadoFactura.Borrador, factura.Estado);
        Assert.Empty(factura.Detalles);
        Assert.Equal(0m, factura.Total);
        Assert.Equal(DateTimeKind.Utc, factura.FechaEmision.Kind);
    }

    [Fact]
    public void Totales_SumanTodosLosDetalles()
    {
        var factura = new Factura(1);
        factura.AgregarDetalle(new DetalleFactura(1, 2, 100m, .18m));
        factura.AgregarDetalle(new DetalleFactura(2, 3, 50m, 0m));
        Assert.Equal(350m, factura.Subtotal);
        Assert.Equal(36m, factura.Itbis);
        Assert.Equal(386m, factura.Total);
        Assert.Equal(5, factura.CantidadTotalProductos);
    }

    [Fact]
    public void Emitir_SinDetallesNoCambiaEstado()
    {
        var factura = new Factura(1);
        Assert.Throws<ConflictoNegocioException>(factura.EmitirFactura);
        Assert.Equal(EstadoFactura.Borrador, factura.Estado);
    }

    [Fact]
    public void Emitir_SoloUnaVezYBloqueaDetalles()
    {
        var factura = new Factura(1);
        var detalle = new DetalleFactura(1, 1, 100m, .18m);
        factura.AgregarDetalle(detalle);
        factura.EmitirFactura();
        Assert.Equal(EstadoFactura.Emitida, factura.Estado);
        Assert.Throws<ConflictoNegocioException>(factura.EmitirFactura);
        Assert.Throws<ConflictoNegocioException>(() => factura.AgregarDetalle(detalle));
        Assert.Single(factura.Detalles);
    }

    [Fact]
    public void AgregarDetalle_RechazaNull() =>
        Assert.Throws<ArgumentException>(() => new Factura(1).AgregarDetalle(null!));

    [Fact]
    public void Detalles_NoPermiteModificarLaColeccion()
    {
        var factura = new Factura(1);
        Assert.Throws<NotSupportedException>(() =>
            ((ICollection<DetalleFactura>)factura.Detalles).Add(new DetalleFactura(1, 1, 1m, 0m)));
    }
}
