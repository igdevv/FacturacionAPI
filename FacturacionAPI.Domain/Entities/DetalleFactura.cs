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
        public decimal TasaItbis { get; private set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal Itbis => Subtotal * TasaItbis;
        public decimal Total => Subtotal + Itbis;

        public DetalleFactura(int productoId, int cantidad, decimal precioUnitario, decimal tasaItbis)
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

            if (tasaItbis < 0 || tasaItbis > 1)
            {
                throw new ArgumentException("La tasa de ITBIS debe estar entre 0 y 1.");
            }

            ProductoId = productoId;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            TasaItbis = tasaItbis;
        }
    }
}
