using BootcampCLTNoviembre.Application.Abstractions.Data;
using BootcampCLTNoviembre.Application.Abstractions.Messaging;

namespace BootcampCLTNoviembre.Application.Products.Queries;

public sealed record GetProductsQuery(string? Search) : IQuery<IReadOnlyList<ProductResponse>>;

internal sealed class GetProductsQueryHandler(IProductsReadStore readStore)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    public Task<IReadOnlyList<ProductResponse>> Handle(GetProductsQuery query, CancellationToken ct)
        => readStore.ListAsync(query.Search, ct);
}
