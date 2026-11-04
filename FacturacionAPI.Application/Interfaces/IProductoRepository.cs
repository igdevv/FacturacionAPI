using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Interfaces
{
    public interface IProductoRepository
    {
        Task AgregarAsync(Producto producto);
        Task<List<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(Producto producto);
        Task<List<Producto>> BuscarPorNombreAsync(string nombre);
    }
}
