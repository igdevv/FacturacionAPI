using FacturacionAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Domain.Entities
{
    public class Factura
    {
        public int Id { get; private set; }
        public int ClienteId { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public List<DetalleFactura> _detalles { get; private set; } = new();
        public IReadOnlyCollection<DetalleFactura> Detalles => _detalles.AsReadOnly();
        public decimal Subtotal => _detalles.Sum(d => d.Subtotal);
        public int CantidadTotalProductos => _detalles.Sum(d => d.Cantidad);
        public EstadoFactura Estado { get; private set; }

        public Factura(int clienteId)
        {
              if (clienteId <= 0)
            {
                throw new ArgumentException("El ID del cliente debe ser mayor que cero.");
            }

            ClienteId = clienteId;
            FechaEmision = DateTime.UtcNow;
        }

        public void AgregarDetalle(DetalleFactura detalle)
        {
            if (detalle is null)
            {
                throw new ArgumentException(nameof(detalle));
            }

            if (Estado != EstadoFactura.Borrador)
            {
                throw new InvalidOperationException(
                    "No se pueden agregar detalles a una factura que no está en borrador.");
            }

            _detalles.Add(detalle);
        }

        public void EmitirFactura()
        {
            if (!_detalles.Any())
            {
                throw new InvalidOperationException("No se puede emitir una factura sin detalles.");
            }

            Estado = EstadoFactura.Emitida;
        }


    }
}
