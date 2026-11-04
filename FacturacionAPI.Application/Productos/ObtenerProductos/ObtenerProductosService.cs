using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Application.Productos.DTOs;
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

        public async Task<List<ProductoResponse>> EjecutarAsync()
        {
            var productos = await _productoRepository.ObtenerTodosAsync();

            return productos.Select(p => new ProductoResponse
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Stock = p.Stock
            }).ToList();
        }
    }
}