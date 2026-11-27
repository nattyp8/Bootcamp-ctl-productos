using BootcampCLTNoviembre.Application.Abstractions.Messaging;

namespace BootcampCLTNoviembre.Infrastructure.Persistence;

/// <summary>Protege toda la operación sobre la colección compartida.</summary>
internal sealed class InMemoryAccessBehavior<TRequest, TResponse>(InMemoryProducts data)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        await data.Gate.WaitAsync(ct);
        try
        {
            return await next();
        }
        finally
        {
            data.Gate.Release();
        }
    }
}
