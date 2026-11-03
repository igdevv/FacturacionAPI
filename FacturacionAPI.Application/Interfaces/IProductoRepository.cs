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
    }
}
