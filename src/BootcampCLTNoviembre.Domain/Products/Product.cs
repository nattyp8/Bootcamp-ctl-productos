
namespace BootcampCLTNoviembre.Domain.Products;

/// <summary>Entidad que protege las reglas al crear o editar un producto.</summary>
public sealed class Product
{
    public const int NameMaxLength = 150;
    public const int DescriptionMaxLength = 500;

    private Product() { }

    public int Id { get; private init; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Product Create(int id, string name, string? description, decimal price, int stock, DateTime now)
    {
        var product = new Product { Id = id, CreatedAt = now };
        product.SetData(name, description, price, stock);
        return product;
    }

    public void Update(string name, string? description, decimal price, int stock, DateTime now)
    {
        SetData(name, description, price, stock);
        UpdatedAt = now;
    }

    private void SetData(string name, string? description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del producto es obligatorio.");
        if (price < 0)
            throw new DomainException("El precio no puede ser negativo.");
        if (stock < 0)
            throw new DomainException("El stock no puede ser negativo.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = price;
        Stock = stock;
    }
}
