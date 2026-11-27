namespace BootcampCLTNoviembre.Domain.Products;

/// <summary>
/// Repositorio del LADO DE ESCRITURA: solo carga y guarda el agregado para aplicar reglas.
/// Las lecturas van por un modelo de lectura separado (IProductsReadStore en Application).
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Indica si ya existe otro producto con ese nombre (sin distinguir mayusculas).</summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);

    int NextId();
    void Add(Product product);
    void Remove(Product product);
}
