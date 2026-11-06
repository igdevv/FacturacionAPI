namespace FacturacionAPI.Domain.Excepciones;

public class ConflictoNegocioException(string mensaje) : Exception(mensaje);
