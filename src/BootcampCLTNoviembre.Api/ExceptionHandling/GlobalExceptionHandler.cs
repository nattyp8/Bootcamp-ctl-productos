using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BootcampCLTNoviembre.Application.Common;
using BootcampCLTNoviembre.Domain;

namespace BootcampCLTNoviembre.Api.ExceptionHandling;

/// <summary>
/// Traduce excepciones a ProblemDetails (RFC 9457):
/// ValidationException -> 400, DomainException -> 400, NotFoundException -> 404,
/// resto -> 500 sin detalle.
/// </summary>
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        ProblemDetails problem = exception switch
        {
            ValidationException ve => new HttpValidationProblemDetails(
                ve.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Datos de entrada invalidos"
            },
            BadHttpRequestException bad => new ProblemDetails
            {
                Status = bad.StatusCode, Title = "Solicitud invalida", Detail = bad.Message
            },
            NotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound, Title = "Recurso no encontrado", Detail = exception.Message
            },
            DomainException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest, Title = "Solicitud invalida", Detail = exception.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError, Title = "Error interno"
            }
        };

        if (problem.Status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado");

        context.Response.StatusCode = problem.Status!.Value;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = problem
        });
    }
}
