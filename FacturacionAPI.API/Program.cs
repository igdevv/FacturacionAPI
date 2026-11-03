using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Application.Productos.CrearProducto;
using FacturacionAPI.Application.Productos.ObtenerProductos;
using FacturacionAPI.Infrastructure.Persistence;
using FacturacionAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddScoped<CrearProductoService>();
builder.Services.AddScoped<ObtenerProductosService>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
