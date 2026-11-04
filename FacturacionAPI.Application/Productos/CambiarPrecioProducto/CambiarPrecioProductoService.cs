using FacturacionAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.CambiarPrecioProducto
{
    public class CambiarPrecioProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public CambiarPrecioProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }


        public async Task<bool> EjecutarAsync(int id, CambiarPrecioProductoDto dto)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id);

            if (producto is null)
                return false;

            producto.CambiarPrecio(dto.NuevoPrecio);

            await _productoRepository.ActualizarAsync(producto);

            return true;

        }
    }
}