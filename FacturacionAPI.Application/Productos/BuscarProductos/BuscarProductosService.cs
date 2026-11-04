using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Application.Productos.DTOs;
using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.BuscarProductos
{
    public class BuscarProductosService
    {
        private readonly IProductoRepository _productoRepository;

        public BuscarProductosService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<List<ProductoResponse>> EjecutarAsync(string nombre)
        {
            var productos = await _productoRepository.BuscarPorNombreAsync(nombre);

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
