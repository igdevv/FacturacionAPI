using FacturacionAPI.Domain.Entities;

namespace FacturacionAPI.Domain.Tests;

public class DetalleFacturaTests
{
    [Theory]
    [InlineData(0.18, 36, 236)]
    [InlineData(0, 0, 200)]
    [InlineData(1, 200, 400)]
    public void CalculaImportes(decimal tasa, decimal itbis, decimal total)
    {
        var detalle = new DetalleFactura(1, 2, 100m, tasa);
        Assert.Equal(200m, detalle.Subtotal);
        Assert.Equal(itbis, detalle.Itbis);
        Assert.Equal(total, detalle.Total);
    }

    [Theory]
    [InlineData(0, 1, 10, .18)]
    [InlineData(1, 0, 10, .18)]
    [InlineData(1, -1, 10, .18)]
    [InlineData(1, 1, 0, .18)]
    [InlineData(1, 1, -1, .18)]
    [InlineData(1, 1, 10, -.01)]
    [InlineData(1, 1, 10, 1.01)]
    public void RechazaDatosInvalidos(int productoId, int cantidad, decimal precio, decimal tasa) =>
        Assert.Throws<ArgumentException>(() => new DetalleFactura(productoId, cantidad, precio, tasa));
}
