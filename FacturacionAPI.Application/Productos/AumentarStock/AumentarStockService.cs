using FacturacionAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.AumentarStock
{
    public class AumentarStockService
    {
        private readonly IProductoRepository _productoRepository;

        public AumentarStockService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<bool> EjecutarAsync(int id, AumentarStockDto dto)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(id);

            if (producto is null)
                return false;

            producto.AumentarStock(dto.Cantidad);

            await _productoRepository.ActualizarAsync(producto);

            return true;
        }
    }
}
