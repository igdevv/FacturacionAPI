using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FacturacionAPI.Infrastructure.Repositories;

public class FacturaRepository(AppDbContext dbContext) : IFacturaRepository
{
    public async Task AgregarAsync(Factura factura)
    {
        dbContext.Facturas.Add(factura);
        // SaveChanges usa una transacción para guardar cabecera y detalles juntos.
        await dbContext.SaveChangesAsync();
    }

    private IQueryable<Factura> ConDetalles() => dbContext.Facturas
        .Include(f => f.Cliente)
        .Include(f => f.Detalles).ThenInclude(d => d.Producto);

    public Task<List<Factura>> ObtenerTodosAsync() => ConDetalles().AsNoTracking()
        .OrderByDescending(f => f.Id).ToListAsync();

    public Task<Factura?> ObtenerPorIdAsync(int id, bool seguimiento = false)
    {
        var consulta = ConDetalles();
        if (!seguimiento)
            consulta = consulta.AsNoTracking();
        return consulta.SingleOrDefaultAsync(f => f.Id == id);
    }

    public Task GuardarCambiosAsync() => dbContext.SaveChangesAsync();
}
