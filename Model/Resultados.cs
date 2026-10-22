namespace DocuTrack.Model;

/// <summary>Caso de eliminación ejecutado, para informarlo en la vista.</summary>
public enum CasoEliminacion
{
    NoEncontrado,
    Hoja,
    UnHijo,
    DosHijos
}

public record ResultadoInsercion(bool Exito, int Comparaciones, string Mensaje);

public record ResultadoBusqueda(bool Encontrado, Nodo? Nodo, int Comparaciones);

public class ResultadoEliminacion
{
    public bool Exito { get; set; }
    public CasoEliminacion Caso { get; set; } = CasoEliminacion.NoEncontrado;
    public int Comparaciones { get; set; }

    /// <summary>Nombre del sucesor que ocupó el lugar (solo caso dos hijos).</summary>
    public string? NombreSucesor { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}

public record ResultadoActualizacion(
    bool Exito,
    string Mensaje,
    ResultadoEliminacion? Eliminacion,
    ResultadoInsercion? Insercion);
