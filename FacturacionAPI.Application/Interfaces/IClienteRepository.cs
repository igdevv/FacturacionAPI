using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task AgregarAsync(Cliente cliente);
        Task<bool> ExistePorDocumentoAsync(string rncCedula);
        Task<List<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
    }
}
