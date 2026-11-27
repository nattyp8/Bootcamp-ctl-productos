using BootcampCLTNoviembre.Domain.Products;

namespace BootcampCLTNoviembre.Application.Products;

/// <summary>DTO de salida: la API nunca expone la entidad de dominio.</summary>
public record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

internal static class ProductMappings
{
    public static ProductResponse ToResponse(this Product p)
        => new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt, p.UpdatedAt);
}
