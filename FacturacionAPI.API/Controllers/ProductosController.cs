using FacturacionAPI.Application.Productos.CrearProducto;
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

        public ProductosController(CrearProductoService crearProductoService, ObtenerProductosService obtenerProductos)
        {
            _crearProductoService = crearProductoService;
            _obtenerProductos = obtenerProductos;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearProductoDto dto)
        {
            var producto = await _crearProductoService.EjecutarAsync(dto);

            return StatusCode(StatusCodes.Status201Created, producto);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var productos = await _obtenerProductos.EjecutarAsync();

            return Ok(productos);
        }
    }
}
