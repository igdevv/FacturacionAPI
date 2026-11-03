using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Domain.Entities
{
    public class DetalleFactura
    {
        public int Id { get; private set; }
        public int ProductoId { get; private set; }
        public int FacturaId { get; private set; }
        public int Cantidad { get; private set; }
        public decimal PrecioUnitario { get; private set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public DetalleFactura(int productoId, int cantidad, decimal precioUnitario)
        {
            if (productoId <= 0)
            {
                throw new ArgumentException("El ID del producto debe ser mayor que cero.");
            }

            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            }

            if (precioUnitario <= 0)
            {
                throw new ArgumentException("El precio unitario debe ser mayor que cero.");
            }

            ProductoId = productoId;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }
}
