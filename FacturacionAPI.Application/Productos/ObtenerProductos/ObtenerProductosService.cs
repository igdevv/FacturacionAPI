using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.ObtenerProductos
{
    public class ObtenerProductosService
    {
        private readonly IProductoRepository _productoRepository;

        public ObtenerProductosService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<List<Producto>> EjecutarAsync()
        {
            return await _productoRepository.ObtenerTodosAsync();
        }
    }
}
