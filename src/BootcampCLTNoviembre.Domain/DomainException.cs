namespace BootcampCLTNoviembre.Domain;

/// <summary>Violacion de una regla de negocio del dominio (HTTP 400).</summary>
public class DomainException(string message) : Exception(message);
