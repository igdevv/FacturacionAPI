using System.Text.Json;
using FacturacionAPI.API.Exceptions;
using FacturacionAPI.Application.Excepciones;
using FacturacionAPI.Domain.Excepciones;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace FacturacionAPI.Domain.Tests;

public class GlobalExceptionHandlerTests
{
    [Theory]
    [InlineData("validacion", 400)]
    [InlineData("inexistente", 404)]
    [InlineData("negocio", 409)]
    [InlineData("concurrencia", 409)]
    [InlineData("interno", 500)]
    public async Task DevuelveProblemDetailsConEstadoCorrecto(string tipo, int estado)
    {
        using var services = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        var contexto = new DefaultHttpContext { RequestServices = services };
        contexto.Response.Body = new MemoryStream();
        Exception excepcion = tipo switch
        {
            "validacion" => new ArgumentException("Datos inválidos"),
            "inexistente" => new RecursoNoEncontradoException("No existe"),
            "negocio" => new ConflictoNegocioException("Conflicto"),
            "concurrencia" => new DbUpdateConcurrencyException(),
            _ => new InvalidOperationException("detalle interno sensible")
        };
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance,
            services.GetRequiredService<IProblemDetailsService>());

        Assert.True(await handler.TryHandleAsync(contexto, excepcion, default));
        Assert.Equal(estado, contexto.Response.StatusCode);
        Assert.StartsWith("application/problem+json", contexto.Response.ContentType);
        contexto.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(contexto.Response.Body);
        Assert.Equal(estado, json.RootElement.GetProperty("status").GetInt32());
        if (estado == 500)
            Assert.DoesNotContain("sensible", json.RootElement.GetProperty("detail").GetString());
    }
}
