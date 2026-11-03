using System;
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
                throw new InvalidOperationException("El precio no puede cero.");
            }

            if (stock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo.");
            }

            Nombre = nombre;
            Precio = precio;
            Stock = stock;
        }

        public void DescontarStock(int cantidad)
        {
            if (cantidad <= 0 || cantidad > Stock)
            {
                throw new InvalidOperationException("Cantidad inválida o inventario insuficiente.");
            }

            Stock -= cantidad;
        }

        public void AumentarStock(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad debe ser mayor que cero");
            }

            Stock += cantidad;
        }

        public void CambiarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
            {
                throw new ArgumentException("El nuevo precio debe ser mayor que cero.");
            }

            Precio = nuevoPrecio;
        }
        
    }
}
