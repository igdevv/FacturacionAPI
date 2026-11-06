using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Interfaces
{
    public interface IFacturaRepository
    {
        Task AgregarAsync(Factura factura);
        Task<List<Factura>> ObtenerTodosAsync();
        Task<Factura?> ObtenerPorIdAsync(int id, bool seguimiento = false);
        Task GuardarCambiosAsync();
    }
}
