using FacturacionAPI.Application.Excepciones;
using FacturacionAPI.Application.Interfaces;

namespace FacturacionAPI.Application.Facturas;

public class EmitirFacturaService(IFacturaRepository facturas)
{
    public async Task EjecutarAsync(int id)
    {
        var factura = await facturas.ObtenerPorIdAsync(id, seguimiento: true)
            ?? throw new RecursoNoEncontradoException("La factura no existe.");
        factura.EmitirFactura();
        await facturas.GuardarCambiosAsync();
    }
}
