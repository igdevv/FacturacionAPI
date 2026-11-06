using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Excepciones;

namespace FacturacionAPI.Domain.Tests;

public class ProductoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Crear_NombreInvalido(string? nombre) =>
        Assert.Throws<ArgumentException>(() => new Producto(nombre!, 10m, 0));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Crear_PrecioInvalido(decimal precio) =>
        Assert.Throws<ArgumentException>(() => new Producto("Mouse", precio, 0));

    [Fact]
    public void Crear_StockNegativo() =>
        Assert.Throws<ArgumentException>(() => new Producto("Mouse", 10m, -1));

    [Fact]
    public void Precio_NoPermiteRedondeoSilencioso() =>
        Assert.Throws<ArgumentException>(() => new Producto("Mouse", 1.001m, 0));

    [Fact]
    public void Stock_AumentaYDescuenta()
    {
        var producto = new Producto("Mouse", 10m, 5);
        producto.AumentarStock(3);
        Assert.Equal(8, producto.Stock);
        producto.DescontarStock(8);
        Assert.Equal(0, producto.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Stock_CantidadInvalidaNoModifica(int cantidad)
    {
        var producto = new Producto("Mouse", 10m, 5);
        Assert.Throws<ArgumentException>(() => producto.AumentarStock(cantidad));
        Assert.Throws<ArgumentException>(() => producto.DescontarStock(cantidad));
        Assert.Equal(5, producto.Stock);
    }

    [Fact]
    public void Descontar_StockInsuficienteNoModifica()
    {
        var producto = new Producto("Mouse", 10m, 5);
        Assert.Throws<ConflictoNegocioException>(() => producto.DescontarStock(6));
        Assert.Equal(5, producto.Stock);
    }

    [Fact]
    public void Aumentar_NoDesbordaEntero()
    {
        var producto = new Producto("Mouse", 10m, int.MaxValue);
        Assert.Throws<ArgumentException>(() => producto.AumentarStock(1));
        Assert.Equal(int.MaxValue, producto.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CambiarPrecio_InvalidoNoModifica(decimal nuevoPrecio)
    {
        var producto = new Producto("Mouse", 1500m, 10);
        Assert.Throws<ArgumentException>(() => producto.CambiarPrecio(nuevoPrecio));
        Assert.Equal(1500m, producto.Precio);
    }
}
