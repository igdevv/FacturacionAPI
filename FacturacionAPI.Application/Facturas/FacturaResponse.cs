using FacturacionAPI.Application.Clientes.DTOs;
using FacturacionAPI.Domain.Entities;

namespace FacturacionAPI.Application.Facturas;

public class FacturaResponse
{
    public int Id { get; init; }
    public ClienteResponse Cliente { get; init; } = new();
    public DateTime FechaEmision { get; init; }
    public string Estado { get; init; } = string.Empty;
    public IReadOnlyList<DetalleFacturaResponse> Detalles { get; init; } = [];
    public decimal Subtotal { get; init; }
    public decimal Itbis { get; init; }
    public decimal Total { get; init; }
    public int CantidadTotalProductos { get; init; }

    public static FacturaResponse DesdeEntidad(Factura factura, Cliente? cliente = null,
        IReadOnlyDictionary<int, Producto>? productos = null)
    {
        cliente ??= factura.Cliente;
        return new FacturaResponse
        {
            Id = factura.Id,
            Cliente = new ClienteResponse
            {
                Id = cliente.Id, Nombre = cliente.Nombre, RncCedula = cliente.RncCedula,
                Correo = cliente.Correo, Telefono = cliente.Telefono
            },
            FechaEmision = factura.FechaEmision,
            Estado = factura.Estado.ToString(),
            Subtotal = factura.Subtotal, Itbis = factura.Itbis, Total = factura.Total,
            CantidadTotalProductos = factura.CantidadTotalProductos,
            Detalles = factura.Detalles.OrderBy(d => d.Id).Select(d => new DetalleFacturaResponse
            {
                Id = d.Id, ProductoId = d.ProductoId,
                NombreProducto = productos is null ? d.Producto.Nombre : productos[d.ProductoId].Nombre,
                Cantidad = d.Cantidad, PrecioUnitario = d.PrecioUnitario, TasaItbis = d.TasaItbis,
                Subtotal = d.Subtotal, Itbis = d.Itbis, Total = d.Total
            }).ToList()
        };
    }
}

public class DetalleFacturaResponse
{
    public int Id { get; init; }
    public int ProductoId { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal TasaItbis { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Itbis { get; init; }
    public decimal Total { get; init; }
}
