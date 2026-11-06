using FacturacionAPI.Application.Clientes.CrearCliente;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Excepciones;
using Microsoft.Data.SqlClient;
using FacturacionAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _dbContext;

        public ClienteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AgregarAsync(Cliente cliente)
        {
            _dbContext.Clientes.Add(cliente);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new ConflictoNegocioException("Ya existe un cliente con ese documento.");
            }
        }

        public async Task<bool> ExistePorDocumentoAsync(string rncCedula)
        {
            return await _dbContext.Clientes
                .AnyAsync(c => c.RncCedula == rncCedula);
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _dbContext.Clientes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            var cliente = await _dbContext.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return cliente;
        }
    }
}
