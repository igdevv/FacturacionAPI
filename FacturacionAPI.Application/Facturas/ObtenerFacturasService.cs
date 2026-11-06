using FacturacionAPI.Application.Interfaces;

namespace FacturacionAPI.Application.Facturas;

public class ObtenerFacturasService(IFacturaRepository facturas)
{
    public async Task<List<FacturaResponse>> ObtenerTodosAsync() =>
        (await facturas.ObtenerTodosAsync()).Select(f => FacturaResponse.DesdeEntidad(f)).ToList();

    public async Task<FacturaResponse?> ObtenerPorIdAsync(int id)
    {
        var factura = await facturas.ObtenerPorIdAsync(id);
        return factura is null ? null : FacturaResponse.DesdeEntidad(factura);
    }
}
