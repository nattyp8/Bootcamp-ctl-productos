using BootcampCLTNoviembre.Application.Abstractions.Messaging;
using BootcampCLTNoviembre.Application.Products.Commands.CreateProduct;
using BootcampCLTNoviembre.Application.Products.Commands.DeleteProduct;
using BootcampCLTNoviembre.Application.Products.Commands.PatchProduct;
using BootcampCLTNoviembre.Application.Products.Commands.UpdateProduct;
using BootcampCLTNoviembre.Application.Products.Queries;

namespace BootcampCLTNoviembre.Api.Endpoints;

/// <summary>Cuerpo del PUT (el id viaja en la ruta).</summary>
public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, int Stock);

/// <summary>Cuerpo del PATCH: solo se modifican los campos enviados.</summary>
public sealed record PatchProductRequest(string? Name, string? Description, decimal? Price, int? Stock);

/// <summary>
/// La presentacion solo conoce ISender: GET -> Query; POST/PUT/PATCH/DELETE -> Command.
/// No sabe que handler se ejecuta, que behaviors lo envuelven ni cómo se almacenan los datos.
/// </summary>
public static class ProductsEndpoints
{
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/api/products").WithTags("Products");

        // ---------- Queries ----------
        group.MapGet("/", async (string? search, ISender sender, CancellationToken ct)
                => TypedResults.Ok(await sender.Send(new GetProductsQuery(search), ct)))
            .WithName("GetProducts")
            .WithSummary("Lista los productos (filtro opcional por nombre).");

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct)
                => TypedResults.Ok(await sender.Send(new GetProductByIdQuery(id), ct)))
            .WithName("GetProductById")
            .WithSummary("Obtiene un producto por id.")
            .ProducesProblem(StatusCodes.Status404NotFound);

        // ---------- Commands ----------
        group.MapPost("/", async (CreateProductCommand command, ISender sender, CancellationToken ct) =>
            {
                var product = await sender.Send(command, ct);
                return TypedResults.CreatedAtRoute(product, "GetProductById", new { id = product.Id });
            })
            .WithName("CreateProduct")
            .WithSummary("Crea un producto.")
            .ProducesValidationProblem();

        group.MapPut("/{id:int}", async (int id, UpdateProductRequest body, ISender sender, CancellationToken ct)
                => TypedResults.Ok(await sender.Send(
                    new UpdateProductCommand(id, body.Name, body.Description, body.Price, body.Stock), ct)))
            .WithName("UpdateProduct")
            .WithSummary("Reemplaza todos los datos de un producto.")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:int}", async (int id, PatchProductRequest body, ISender sender, CancellationToken ct)
                => TypedResults.Ok(await sender.Send(
                    new PatchProductCommand(id, body.Name, body.Description, body.Price, body.Stock), ct)))
            .WithName("PatchProduct")
            .WithSummary("Modifica solo los campos enviados.")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new DeleteProductCommand(id), ct);
                return TypedResults.NoContent();
            })
            .WithName("DeleteProduct")
            .WithSummary("Elimina un producto.")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
