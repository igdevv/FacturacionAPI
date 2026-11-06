using FacturacionAPI.Application.Clientes.CrearCliente;
using FacturacionAPI.Application.Clientes.ObtenerClientesPorId;
using FacturacionAPI.Application.Clientes.ObtenerTodosLosClientes;
using Microsoft.AspNetCore.Mvc;

namespace FacturacionAPI.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController: ControllerBase
    {
        private readonly CrearClienteService _clienteService;
        private readonly ObtenerClientePorIdService _obtenerClientePorId;
        private readonly ObtenerTodosLosClientesService _obtenerTodosLosClientes;

        public ClientesController(CrearClienteService clienteService, ObtenerClientePorIdService obtenerClientePorId,
            ObtenerTodosLosClientesService obtenerTodosLosClientes)
        {
            _clienteService = clienteService;
            _obtenerClientePorId = obtenerClientePorId;
            _obtenerTodosLosClientes = obtenerTodosLosClientes;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearClienteDto dto)
        {
            var cliente = await _clienteService.EjecutarAsync(dto);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var cliente = await _obtenerTodosLosClientes.ObtenerTodosAsync();

            return Ok(cliente);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cliente = await _obtenerClientePorId.ObtenerPorIdAsync(id);

            if (cliente is null)
                return NotFound();

            return Ok(cliente);
        }
    }
}
