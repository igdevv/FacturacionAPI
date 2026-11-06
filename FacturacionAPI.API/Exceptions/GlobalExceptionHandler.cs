using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FacturacionAPI.Application.Excepciones;
using FacturacionAPI.Domain.Excepciones;
using Microsoft.EntityFrameworkCore;

namespace FacturacionAPI.API.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problems) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                ArgumentException => (400, "Datos inválidos", exception.Message),
                RecursoNoEncontradoException => (404, "Recurso no encontrado", exception.Message),
                ConflictoNegocioException => (409, "Conflicto de negocio", exception.Message),
                DbUpdateConcurrencyException => (409, "Conflicto de concurrencia",
                    "El recurso fue modificado por otra operación. Consulta su estado y vuelve a intentarlo."),
                _ => (500, "Error interno", "Ocurrió un error inesperado al procesar la solicitud.")
            };
            if (status == 500)
                logger.LogError(exception, "Error al procesar {Ruta}", httpContext.Request.Path);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = status;

            await problems.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext, ProblemDetails = problem
            });

            return true;
        }
    }
}
