namespace BootcampCLTNoviembre.Application.Common;

/// <summary>El recurso solicitado no existe (HTTP 404).</summary>
public class NotFoundException(string message) : Exception(message)
{
    public static NotFoundException Product(int id) => new($"No existe el producto {id}.");
}
