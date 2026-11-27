using BootcampCLTNoviembre.Application.Abstractions.Messaging;
using BootcampCLTNoviembre.Application.Common;
using BootcampCLTNoviembre.Domain.Products;
using FluentValidation;

namespace BootcampCLTNoviembre.Application.Products.Commands.PatchProduct;

/// <summary>Actualizacion parcial (PATCH): los campos null conservan su valor actual.</summary>
public sealed record PatchProductCommand(int Id, string? Name, string? Description, decimal? Price, int? Stock)
    : ICommand<ProductResponse>;

internal sealed class PatchProductCommandValidator : AbstractValidator<PatchProductCommand>
{
    public PatchProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x)
            .Must(x => x.Name is not null || x.Description is not null || x.Price is not null || x.Stock is not null)
            .OverridePropertyName("Body")
            .WithMessage("Debe enviar al menos un campo a modificar.");

        RuleFor(x => x.Name!).ValidName().OverridePropertyName("Name").When(x => x.Name is not null);
        RuleFor(x => x.Description).ValidDescription();
        RuleFor(x => x.Price!.Value).ValidPrice().OverridePropertyName("Price").When(x => x.Price is not null);
        RuleFor(x => x.Stock!.Value).ValidStock().OverridePropertyName("Stock").When(x => x.Stock is not null);
    }
}

internal sealed class PatchProductCommandHandler(
    IProductRepository repository,
    TimeProvider timeProvider) : ICommandHandler<PatchProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(PatchProductCommand command, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(command.Id, ct) ?? throw NotFoundException.Product(command.Id);

        if (command.Name is not null)
            await repository.EnsureUniqueNameAsync(command.Name, excludeId: product.Id, ct);

        // El agregado sigue validando sus invariantes: recibe el estado resultante completo.
        product.Update(
            command.Name ?? product.Name,
            command.Description ?? product.Description,
            command.Price ?? product.Price,
            command.Stock ?? product.Stock,
            timeProvider.GetUtcNow().UtcDateTime);

        return product.ToResponse();
    }
}
