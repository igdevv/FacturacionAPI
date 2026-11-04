using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Application.Productos.DTOs;
using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.ObtenerProductoPorId
{
    public class ObtenerProductoPorIdService
    {
        private readonly IProductoRepository _productoRepository;

        public ObtenerProductoPorIdService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<ProductoResponse?> EjecutarAsync(int id)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id);

            if (producto is null)
                return null;

            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }
    }
}
