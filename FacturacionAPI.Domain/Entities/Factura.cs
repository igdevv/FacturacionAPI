using FacturacionAPI.Domain.Enums;
using FacturacionAPI.Domain.Excepciones;
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
        private readonly List<DetalleFactura> _detalles = new();
        public Cliente Cliente { get; private set; } = null!;
        public IReadOnlyCollection<DetalleFactura> Detalles => _detalles.AsReadOnly();
        public decimal Subtotal => _detalles.Sum(d => d.Subtotal);
        public decimal Itbis => _detalles.Sum(d => d.Itbis);
        public decimal Total => Subtotal + Itbis;
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
                throw new ConflictoNegocioException(
                    "No se pueden agregar detalles a una factura que no está en borrador.");
            }

            _detalles.Add(detalle);
        }

        public void EmitirFactura()
        {
            if (Estado != EstadoFactura.Borrador)
            {
                throw new ConflictoNegocioException("Solo se pueden emitir facturas en estado Borrador.");
            }

            if (!_detalles.Any())
            {
                throw new ConflictoNegocioException("No se puede emitir una factura sin detalles.");
            }

            if (Total <= 0)
            {
                throw new ConflictoNegocioException("No se puede emitir una factura con total cero o negativo.");
            }

            Estado = EstadoFactura.Emitida;
        }


    }
}
