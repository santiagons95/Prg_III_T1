namespace DocuTrack.Model;

/// <summary>Nodo del repositorio: una carpeta o un archivo.</summary>
public class Nodo
{
    public string Nombre { get; set; }
    public bool EsCarpeta { get; set; }
    public Nodo? Izquierdo { get; set; }
    public Nodo? Derecho { get; set; }

    public Nodo(string nombre, bool esCarpeta)
    {
        Nombre = nombre;
        EsCarpeta = esCarpeta;
    }

    public bool EsHoja => Izquierdo == null && Derecho == null;

    public int CantidadHijos => (Izquierdo != null ? 1 : 0) + (Derecho != null ? 1 : 0);
}
