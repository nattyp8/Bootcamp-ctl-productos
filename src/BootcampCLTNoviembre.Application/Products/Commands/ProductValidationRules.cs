using BootcampCLTNoviembre.Domain.Products;
using FluentValidation;

namespace BootcampCLTNoviembre.Application.Products.Commands;

/// <summary>Reglas de FORMATO compartidas por los validadores de comandos (HTTP 400).</summary>
internal static class ProductValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> rule)
        => rule.NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(Product.NameMaxLength);

    public static IRuleBuilderOptions<T, string?> ValidDescription<T>(this IRuleBuilder<T, string?> rule)
        => rule.MaximumLength(Product.DescriptionMaxLength);

    // Conservamos el formato de precio del ejercicio: hasta 16 enteros y 2 decimales.
    public static IRuleBuilderOptions<T, decimal> ValidPrice<T>(this IRuleBuilder<T, decimal> rule)
        => rule.GreaterThanOrEqualTo(0).PrecisionScale(18, 2, ignoreTrailingZeros: true);

    public static IRuleBuilderOptions<T, int> ValidStock<T>(this IRuleBuilder<T, int> rule)
        => rule.GreaterThanOrEqualTo(0);
}
