using BootcampCLTNoviembre.Application.Abstractions.Data;
using BootcampCLTNoviembre.Application.Products;
using BootcampCLTNoviembre.Domain.Products;
using BootcampCLTNoviembre.Infrastructure.Persistence;

namespace BootcampCLTNoviembre.Infrastructure.ReadModels;

/// <summary>Devuelve copias de los datos para las consultas.</summary>
internal sealed class ProductsReadStore(InMemoryProducts data) : IProductsReadStore
{
    public Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        ProductResponse? response = data.Items.TryGetValue(id, out var product) ? ToResponse(product) : null;
        return Task.FromResult(response);
    }

    public Task<IReadOnlyList<ProductResponse>> ListAsync(string? search, CancellationToken ct = default)
    {
        var term = search?.Trim() ?? string.Empty;
        IReadOnlyList<ProductResponse> response = data.Items.Values
            .Where(product => product.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(product => product.Id)
            .Select(ToResponse)
            .ToArray();
        return Task.FromResult(response);
    }

    private static ProductResponse ToResponse(Product product) => new(
        product.Id, product.Name, product.Description, product.Price,
        product.Stock, product.CreatedAt, product.UpdatedAt);
}
