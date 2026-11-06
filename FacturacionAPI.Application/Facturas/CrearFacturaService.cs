using FacturacionAPI.Application.Excepciones;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;

namespace FacturacionAPI.Application.Facturas;

public class CrearFacturaService(IFacturaRepository facturas, IClienteRepository clientes,
    IProductoRepository productos)
{
    // Política didáctica de V1: todos los detalles usan 18 %, sin clasificación fiscal.
    private const decimal TasaItbis = 0.18m;

    public async Task<FacturaResponse> EjecutarAsync(CrearFacturaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.ClienteId <= 0)
            throw new ArgumentException("El ID del cliente debe ser mayor que cero.");
        if (dto.Productos is null || dto.Productos.Count == 0)
            throw new ArgumentException("La factura debe tener al menos un producto.");
        if (dto.Productos.Any(p => p is null || p.ProductoId <= 0 || p.Cantidad <= 0))
            throw new ArgumentException("Los productos deben tener ID y cantidad mayores que cero.");
        if (dto.Productos.Select(p => p.ProductoId).Distinct().Count() != dto.Productos.Count)
            throw new ArgumentException("No se puede repetir un producto; indica su cantidad en una sola línea.");
        if (dto.Productos.Sum(p => (long)p.Cantidad) > int.MaxValue)
            throw new ArgumentException("La cantidad total de productos excede el máximo permitido.");

        var cliente = await clientes.ObtenerPorIdAsync(dto.ClienteId)
            ?? throw new RecursoNoEncontradoException("El cliente no existe.");
        var ids = dto.Productos.Select(p => p.ProductoId).ToArray();
        var encontrados = (await productos.ObtenerPorIdsAsync(ids)).ToDictionary(p => p.Id);
        var faltantes = ids.Where(id => !encontrados.ContainsKey(id)).ToArray();
        if (faltantes.Length > 0)
            throw new RecursoNoEncontradoException($"Productos inexistentes: {string.Join(", ", faltantes)}.");

        var factura = new Factura(cliente.Id);
        foreach (var linea in dto.Productos)
            factura.AgregarDetalle(new DetalleFactura(linea.ProductoId, linea.Cantidad,
                encontrados[linea.ProductoId].Precio, TasaItbis));

        await facturas.AgregarAsync(factura);
        return FacturaResponse.DesdeEntidad(factura, cliente, encontrados);
    }
}
