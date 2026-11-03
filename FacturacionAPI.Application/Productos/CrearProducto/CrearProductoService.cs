using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.CrearProducto
{
    public class CrearProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public CrearProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }
        
        public async Task<Producto> EjecutarAsync(CrearProductoDto dto)
        {
            var producto = new Producto(dto.Nombre, dto.Precio, dto.Stock);

            await _productoRepository.AgregarAsync(producto);

            return producto;
        }
    }
}
