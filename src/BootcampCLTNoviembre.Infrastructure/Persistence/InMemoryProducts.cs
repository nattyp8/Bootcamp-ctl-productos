using BootcampCLTNoviembre.Domain.Products;

namespace BootcampCLTNoviembre.Infrastructure.Persistence;

/// <summary>Datos compartidos durante la ejecución de la API.</summary>
internal sealed class InMemoryProducts
{
    private int _lastId;

    public Dictionary<int, Product> Items { get; } = new();
    public SemaphoreSlim Gate { get; } = new(1, 1);

    public InMemoryProducts(TimeProvider clock)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var notebook = Product.Create(NextId(), "Notebook", "Notebook para desarrollo", 1200m, 10, now);
        Items.Add(notebook.Id, notebook);

        var mouse = Product.Create(NextId(), "Mouse", "Mouse inalámbrico", 25.50m, 30, now);
        Items.Add(mouse.Id, mouse);
    }

    public int NextId() => ++_lastId;
}
