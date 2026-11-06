using FacturacionAPI.Application.Productos.AumentarStock;
using FacturacionAPI.Application.Productos.BuscarProductos;
using FacturacionAPI.Application.Productos.CambiarPrecioProducto;
using FacturacionAPI.Application.Productos.CrearProducto;
using FacturacionAPI.Application.Productos.DescontarStock;
using FacturacionAPI.Application.Productos.ObtenerProductoPorId;
using FacturacionAPI.Application.Productos.ObtenerProductos;
using Microsoft.AspNetCore.Mvc;

namespace FacturacionAPI.API.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController: ControllerBase
    {
        private readonly CrearProductoService _crearProductoService;
        private readonly ObtenerProductosService _obtenerProductos;
        private readonly ObtenerProductoPorIdService _obtenerProductoPorId;
        private readonly CambiarPrecioProductoService _cambiarPrecioProducto;
        private readonly AumentarStockService _aumentarStock;
        private readonly DescontarStockService _descontarStock;
        private readonly BuscarProductosService _buscarProductos;

        public ProductosController(CrearProductoService crearProductoService, ObtenerProductosService obtenerProductos,
            ObtenerProductoPorIdService obtenerProductoPorId, CambiarPrecioProductoService cambiarPrecioProducto,
            AumentarStockService aumentarStock, DescontarStockService descontarStock, BuscarProductosService buscarProductos)
        {
            _crearProductoService = crearProductoService;
            _obtenerProductos = obtenerProductos;
            _obtenerProductoPorId = obtenerProductoPorId;
            _cambiarPrecioProducto = cambiarPrecioProducto;
            _aumentarStock = aumentarStock;
            _descontarStock = descontarStock;
            _buscarProductos = buscarProductos;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearProductoDto dto)
        {
            var producto = await _crearProductoService.EjecutarAsync(dto);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var productos = await _obtenerProductos.EjecutarAsync();

            return Ok(productos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _obtenerProductoPorId.EjecutarAsync(id);

            if (producto is null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpPatch("{id:int}/precio")]
        public async Task<IActionResult> CambiarPrecio(int id, [FromBody] CambiarPrecioProductoDto dto)
        {
            var actualizado = await _cambiarPrecioProducto.EjecutarAsync(id, dto);

            if (!actualizado)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/stock/aumentar")]
        public async Task<IActionResult> AumentarStock(int id, [FromBody] AumentarStockDto dto)
        {
            var actualizado = await _aumentarStock.EjecutarAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id:int}/stock/descontar")]
        public async Task<IActionResult> DescontarStock(int id, [FromBody] DescontarStockDto dto)
        {
            var producto = await _descontarStock.EjecutarAsync(id, dto);

            if (!producto)
                return NotFound();

            return NoContent();
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorNombre([FromQuery]string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Problem(statusCode: 400, title: "Datos inválidos", detail: "Debes indicar un nombre para buscar.");

            var productos = await _buscarProductos.EjecutarAsync(nombre);

            return Ok(productos);
        }
    }
}
