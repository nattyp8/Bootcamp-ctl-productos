using BootcampCLTNoviembre.Application.Products;

namespace BootcampCLTNoviembre.Application.Abstractions.Data;

/// <summary>
/// Puerto del MODELO DE LECTURA: devuelve DTOs listos para la respuesta, optimizados para consultar.
/// No pasa por el agregado ni aplica reglas de negocio.
/// </summary>
public interface IProductsReadStore
{
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <param name="search">Filtro opcional por nombre (contiene, sin distinguir mayusculas).</param>
    /// <param name="ct">Token de cancelacion.</param>
    Task<IReadOnlyList<ProductResponse>> ListAsync(string? search, CancellationToken ct = default);
}
