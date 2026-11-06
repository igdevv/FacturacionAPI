using FacturacionAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.DescontarStock
{
    public class DescontarStockService
    {
        private readonly IProductoRepository _productoRepository;

        public DescontarStockService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<bool> EjecutarAsync(int id, DescontarStockDto dto)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id, seguimiento: true);

            if (producto is null)
                return false;

            producto.DescontarStock(dto.Cantidad);

            await _productoRepository.ActualizarAsync(producto);

            return true;
        }
    }
}
