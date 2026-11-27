using BootcampCLTNoviembre.Application.Abstractions.Messaging;
using BootcampCLTNoviembre.Application.Common;
using BootcampCLTNoviembre.Domain.Products;

namespace BootcampCLTNoviembre.Application.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int Id) : ICommand<Unit>;

internal sealed class DeleteProductCommandHandler(IProductRepository repository)
    : ICommandHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(command.Id, ct) ?? throw NotFoundException.Product(command.Id);

        repository.Remove(product);
        return Unit.Value;
    }
}
