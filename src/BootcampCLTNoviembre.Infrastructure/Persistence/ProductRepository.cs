using BootcampCLTNoviembre.Domain.Products;

namespace BootcampCLTNoviembre.Infrastructure.Persistence;

/// <summary>Obtiene y guarda entidades del lado de escritura.</summary>
internal sealed class ProductRepository(InMemoryProducts data) : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(data.Items.GetValueOrDefault(id));

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
        => Task.FromResult(data.Items.Values.Any(product =>
            product.Id != excludeId &&
            string.Equals(product.Name, name, StringComparison.OrdinalIgnoreCase)));

    public int NextId() => data.NextId();

    public void Add(Product product) => data.Items.Add(product.Id, product);

    public void Remove(Product product) => data.Items.Remove(product.Id);
}
