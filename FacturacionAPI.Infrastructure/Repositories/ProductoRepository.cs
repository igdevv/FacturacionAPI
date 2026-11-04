using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AgregarAsync(Producto producto)
        {
            await _dbContext.Productos.AddAsync(producto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _dbContext.Productos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            var producto = await _dbContext.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto;
        }

        public async Task ActualizarAsync(Producto producto)
        {
            _dbContext.Productos.Update(producto);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Producto>> BuscarPorNombreAsync(string nombre)
        {
            return await _dbContext.Productos
                .AsNoTracking()
                .Where(p => p.Nombre.Contains(nombre))
                .ToListAsync();
        }
    }
}
