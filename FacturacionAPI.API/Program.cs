using FacturacionAPI.Application.Clientes.CrearCliente;
using FacturacionAPI.Application.Clientes.ObtenerClientes;
using FacturacionAPI.Application.Clientes.ObtenerClientesPorId;
using FacturacionAPI.Application.Clientes.ObtenerTodosLosClientes;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Application.Facturas;
using FacturacionAPI.API.Exceptions;
using FacturacionAPI.Application.Productos.AumentarStock;
using FacturacionAPI.Application.Productos.BuscarProductos;
using FacturacionAPI.Application.Productos.CambiarPrecioProducto;
using FacturacionAPI.Application.Productos.CrearProducto;
using FacturacionAPI.Application.Productos.DescontarStock;
using FacturacionAPI.Application.Productos.ObtenerProductoPorId;
using FacturacionAPI.Application.Productos.ObtenerProductos;
using FacturacionAPI.Infrastructure.Persistence;
using FacturacionAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<CrearFacturaService>();
builder.Services.AddScoped<ObtenerFacturasService>();
builder.Services.AddScoped<EmitirFacturaService>();

builder.Services.AddScoped<CrearProductoService>();
builder.Services.AddScoped<ObtenerProductosService>();
builder.Services.AddScoped<ObtenerProductoPorIdService>();
builder.Services.AddScoped<CambiarPrecioProductoService>();
builder.Services.AddScoped<AumentarStockService>();
builder.Services.AddScoped<DescontarStockService>();
builder.Services.AddScoped<BuscarProductosService>();


builder.Services.AddScoped<CrearClienteService>();
builder.Services.AddScoped<ObtenerClientesService>();
builder.Services.AddScoped<ObtenerClientePorIdService>();
builder.Services.AddScoped<ObtenerTodosLosClientesService>();

builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

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
