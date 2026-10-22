using System.Text;
using DocuTrack.Model;

namespace DocuTrack.View;

/// Presentación en consola. No contiene lógica de negocio
public class ConsolaVista
{
    public ConsolaVista()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    // ------------------------------------------------------------ Encabezados

    public void MostrarTitulo(string titulo)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"  {titulo}");
        Console.WriteLine(new string('=', 70));
    }

    public void MostrarSubtitulo(string subtitulo)
    {
        Console.WriteLine();
        Console.WriteLine($"--- {subtitulo} ---");
    }

    public void MostrarMensaje(string mensaje) => Console.WriteLine(mensaje);

    // ------------------------------------------------------------- Resultados

    public void MostrarInsercion(string nombre, bool esCarpeta, ResultadoInsercion r)
    {
        string estado = r.Exito ? "OK       " : "RECHAZADO";
        Console.WriteLine($"[INSERTAR {estado}] {Tipo(esCarpeta)} '{nombre}' | comparaciones: {r.Comparaciones} | {r.Mensaje}");
    }

    public void MostrarBusqueda(string nombre, ResultadoBusqueda r)
    {
        string estado = r.Encontrado ? "HALLADO   " : "NO HALLADO";
        string detalle = r.Encontrado
            ? $" | {Tipo(r.Nodo!.EsCarpeta)}, {DescribirPosicion(r.Nodo)}"
            : string.Empty;
        Console.WriteLine($"[BUSCAR {estado}] '{nombre}' | comparaciones: {r.Comparaciones}{detalle}");
    }

    public void MostrarEliminacion(ResultadoEliminacion r)
    {
        if (!r.Exito)
        {
            Console.WriteLine($"[ELIMINAR FALLIDO] {r.Mensaje} | comparaciones: {r.Comparaciones}");
            return;
        }

        string caso = r.Caso switch
        {
            CasoEliminacion.Hoja => "Caso 1: nodo HOJA, se desenlaza del padre.",
            CasoEliminacion.UnHijo => "Caso 2: nodo con UN HIJO, el hijo se reconecta con el padre.",
            CasoEliminacion.DosHijos =>
                $"Caso 3: nodo con DOS HIJOS, lo reemplaza su sucesor inorden '{r.NombreSucesor}' y se elimina el nodo movido.",
            _ => "Caso desconocido."
        };
        Console.WriteLine($"[ELIMINAR OK] {r.Mensaje} | comparaciones: {r.Comparaciones}");
        Console.WriteLine($"              {caso}");
    }

    public void MostrarActualizacion(ResultadoActualizacion r)
    {
        if (r.Eliminacion == null || r.Insercion == null)
        {
            Console.WriteLine($"[ACTUALIZAR FALLIDO] {r.Mensaje}");
            return;
        }

        Console.WriteLine($"[ACTUALIZAR {(r.Exito ? "OK" : "FALLIDO")}] {r.Mensaje}  (Eliminar + Insertar)");
        Console.Write("  paso 1 -> ");
        MostrarEliminacion(r.Eliminacion);
        Console.WriteLine($"  paso 2 -> Insertar nuevo nombre | comparaciones: {r.Insercion.Comparaciones} | {r.Insercion.Mensaje}");
    }

    public void MostrarTipoDeNodo(string nombre, Nodo nodo)
    {
        string tipo = nodo.CantidadHijos switch
        {
            0 => "hoja",
            1 => "nodo con un solo hijo",
            _ => "nodo con dos hijos"
        };
        Console.WriteLine($"Objetivo: '{nombre}' ({Tipo(nodo.EsCarpeta)}) -> {tipo}.");
    }

    // ------------------------------------------------------------ Recorridos

    public void MostrarRecorrido(string titulo, IEnumerable<Nodo> nodos)
    {
        Console.WriteLine($"{titulo,-12}: {string.Join(" -> ", nodos.Select(n => n.Nombre))}");
    }

    public void MostrarVerificacionInorden(bool ordenado)
    {
        Console.WriteLine();
        Console.WriteLine(ordenado
            ? "Verificación: el INORDEN está en orden alfabético ascendente -> la propiedad BST se conserva."
            : "Verificación: el INORDEN NO está ordenado -> la propiedad BST está rota.");
    }

    public void MostrarVerificacionArchivos(bool correcto)
    {
        Console.WriteLine(correcto
            ? "Verificación: todos los archivos son hojas."
            : "ADVERTENCIA: existe un archivo con hijos (viola la regla de dominio).");
    }

    // ----------------------------------------------------------- Métricas

    public void MostrarMetricas(int cantidad, int altura)
    {
        Console.WriteLine($"Total de nodos: {cantidad} | Altura del árbol: {altura} niveles");
    }

    // --------------------------------------------------------- Árbol ASCII

    public void MostrarArbol(Nodo? raiz)
    {
        if (raiz == null)
        {
            Console.WriteLine("(árbol vacío)");
            return;
        }

        Console.WriteLine(Descripcion(raiz));
        ImprimirHijos(raiz, string.Empty);
        Console.WriteLine("Leyenda: [C] carpeta, [A] archivo, (izq)/(der) posición del hijo.");
    }

    private static void ImprimirHijos(Nodo padre, string prefijo)
    {
        var hijos = new List<(Nodo Nodo, string Lado)>();
        if (padre.Izquierdo != null) hijos.Add((padre.Izquierdo, "izq"));
        if (padre.Derecho != null) hijos.Add((padre.Derecho, "der"));

        for (int i = 0; i < hijos.Count; i++)
        {
            bool esUltimo = i == hijos.Count - 1;
            var (nodo, lado) = hijos[i];
            Console.WriteLine($"{prefijo}{(esUltimo ? "└── " : "├── ")}({lado}) {Descripcion(nodo)}");
            ImprimirHijos(nodo, prefijo + (esUltimo ? "    " : "│   "));
        }
    }

    // ------------------------------------------------------------ Utilidades

    private static string Descripcion(Nodo n) => $"{n.Nombre} [{(n.EsCarpeta ? "C" : "A")}]";

    private static string Tipo(bool esCarpeta) => esCarpeta ? "carpeta" : "archivo";

    private static string DescribirPosicion(Nodo n) => n.CantidadHijos switch
    {
        0 => "hoja",
        1 => "un hijo",
        _ => "dos hijos"
    };
}
