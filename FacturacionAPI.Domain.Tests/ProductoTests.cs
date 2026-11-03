using FacturacionAPI.Domain.Entities;

namespace FacturacionAPI.Domain.Tests
{
    public class ProductoTests
    {
        //[Fact]

        //public void DescontarStock_CantidadValida_DisminuyeStock()
        //{
        //    // Arrange: preparar los datos
        //    var producto = new Producto("Teclado", 1500m, 10);


        //    // Act: ejecutar la operación
        //    producto.DescontarStock(3);

        //    // Assert: comprobar el resultado
        //    Assert.Equal(7, producto.Stock);
        //}

        //public void DescontarStock_StockInsuficiente_LanzaExcepcion()
        //{
        //    var producto = new Producto("Mouse", 2000m, 10);

        //    //producto.DescontarStock(15);

        //    Assert.Throws<InvalidOperationException>(() =>
        //    {
        //        producto.DescontarStock(15);
        //    });
        //}

        //[Theory]
        //[InlineData(0)]
        //[InlineData(-5)]
        //[InlineData(15)]
        //public void DescontarStock_CantidadInvalida_LanzaExcepcion(int cantidad)
        //{
        //    var producto = new Producto("Mouse", 800m, 10);

        //    Assert.Throws<InvalidOperationException>(() =>
        //    {
        //        producto.DescontarStock(cantidad);
        //    });

        //    Assert.Equal(10, producto.Stock);
        //}

        //[Fact]
        //public void CambiarPrecio_PrecioValido_ActualizarPrecio()
        //{
        //    var producto = new Producto("Mouse", 1500m, 10);

        //    producto.CambiarPrecio(1800);

        //    Assert.Equal(1800m, producto.Precio);
        //}

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void CambiarPrecio_PrecioInvalido_LanzaExcepcion(decimal nuevoPrecio)
        {
            var producto = new Producto("Mouse", 1500m, 10);

            // Aquí debes comprobar que se lanza ArgumentException.
            Assert.Throws<ArgumentException>(() =>
            {
                producto.CambiarPrecio(-100);
            });

            Assert.Equal(1500m, producto.Precio);

            // Después comprueba que el precio sigue siendo 1500.
        }

    }
}

