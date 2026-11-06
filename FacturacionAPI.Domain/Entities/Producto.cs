using System;
using FacturacionAPI.Domain.Excepciones;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Domain.Entities
{
    public class Producto
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public decimal Precio { get; private set; }
        public int Stock { get; private set; }

        public Producto(string nombre, decimal precio, int stock)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre no puede estar vacío.");
            }

            if (precio <= 0)
            {
                throw new ArgumentException("El precio debe ser mayor que cero.");
            }

            if (stock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo.");
            }

            ValidarPrecisionPrecio(precio);
            Nombre = nombre.Trim();
            Precio = precio;
            Stock = stock;
        }

        public void DescontarStock(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            if (cantidad > Stock)
                throw new ConflictoNegocioException("Inventario insuficiente.");

            Stock -= cantidad;
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            if (cantidad > int.MaxValue - Stock)
                throw new ArgumentException("La cantidad excede el stock máximo permitido.");

            Stock += cantidad;
        }

        public void CambiarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
            {
                throw new ArgumentException("El nuevo precio debe ser mayor que cero.");
            }

            ValidarPrecisionPrecio(nuevoPrecio);
            Precio = nuevoPrecio;
        }

        private static void ValidarPrecisionPrecio(decimal precio)
        {
            if (precio > 9999999999999999.99m || decimal.Round(precio, 2) != precio)
                throw new ArgumentException("El precio debe caber en decimal(18,2), con máximo dos decimales.");
        }
        
    }
}
