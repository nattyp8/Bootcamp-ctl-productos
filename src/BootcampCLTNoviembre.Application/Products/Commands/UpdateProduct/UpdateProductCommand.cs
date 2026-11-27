using BootcampCLTNoviembre.Application.Abstractions.Messaging;
using BootcampCLTNoviembre.Application.Common;
using BootcampCLTNoviembre.Domain.Products;
using FluentValidation;

namespace BootcampCLTNoviembre.Application.Products.Commands.UpdateProduct;

/// <summary>Reemplazo completo del producto (PUT).</summary>
public sealed record UpdateProductCommand(int Id, string Name, string? Description, decimal Price, int Stock)
    : ICommand<ProductResponse>;

internal sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).ValidName();
        RuleFor(x => x.Description).ValidDescription();
        RuleFor(x => x.Price).ValidPrice();
        RuleFor(x => x.Stock).ValidStock();
    }
}

internal sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    TimeProvider timeProvider) : ICommandHandler<UpdateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(command.Id, ct) ?? throw NotFoundException.Product(command.Id);

        await repository.EnsureUniqueNameAsync(command.Name, excludeId: product.Id, ct);

        product.Update(command.Name, command.Description, command.Price, command.Stock,
            timeProvider.GetUtcNow().UtcDateTime);

        return product.ToResponse();
    }
}
