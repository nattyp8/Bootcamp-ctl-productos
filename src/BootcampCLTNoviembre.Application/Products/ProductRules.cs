using BootcampCLTNoviembre.Domain;
using BootcampCLTNoviembre.Domain.Products;

namespace BootcampCLTNoviembre.Application.Products;

/// <summary>
/// Reglas que necesitan consultar persistencia (no caben dentro del agregado).
/// Violarlas lanza DomainException (HTTP 400).
/// </summary>
internal static class ProductRules
{
    public static async Task EnsureUniqueNameAsync(
        this IProductRepository repository, string name, int? excludeId, CancellationToken ct)
    {
        var nombre = name.Trim();
        if (await repository.ExistsByNameAsync(nombre, excludeId, ct))
            throw new DomainException($"Ya existe un producto con el nombre '{nombre}'.");
    }
}
