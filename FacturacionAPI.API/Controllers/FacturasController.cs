using FacturacionAPI.Application.Facturas;
using Microsoft.AspNetCore.Mvc;

namespace FacturacionAPI.API.Controllers;

[ApiController]
[Route("api/facturas")]
public class FacturasController(CrearFacturaService crear, ObtenerFacturasService obtener,
    EmitirFacturaService emitir) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Crear(CrearFacturaDto dto)
    {
        var factura = await crear.EjecutarAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = factura.Id }, factura);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() => Ok(await obtener.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var factura = await obtener.ObtenerPorIdAsync(id);
        return factura is null ? NotFound() : Ok(factura);
    }

    [HttpPatch("{id:int}/emitir")]
    public async Task<IActionResult> Emitir(int id)
    {
        await emitir.EjecutarAsync(id);
        return NoContent();
    }
}
