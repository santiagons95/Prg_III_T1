using DocuTrack.Model;
using DocuTrack.View;

namespace DocuTrack.Controller;

/// <summary>
/// Orquesta los casos de uso en el orden solicitado. No contiene lógica estructural
/// del BST: todo recorrido, búsqueda o reestructuración vive en el Model.
/// </summary>
public class DocuTrackController
{
    private readonly ArbolBinario _arbol;
    private readonly ConsolaVista _vista;

    // Dataset: 14 nombres (carpetas y archivos). Las carpetas ocupan los nodos internos
    // para respetar "un archivo es siempre hoja".
    private static readonly (string Nombre, bool EsCarpeta)[] DatosIniciales =
    {
        ("Documentos", true),
        ("Backups", true),
        ("Informes", true),
        ("Actas.pdf", false),
        ("Contratos", true),
        ("Facturas", true),
        ("Proyectos", true),
        ("Clientes", true),
        ("Egresos", true),
        ("Fotos.png", false),
        ("Manuales", true),
        ("Reportes", true),
        ("Nomina.xlsx", false),
        ("Zonas.txt", false),
    };

    public DocuTrackController(ArbolBinario arbol, ConsolaVista vista)
    {
        _arbol = arbol;
        _vista = vista;
    }

    public void Ejecutar()
    {
        ConstruirArbol();
        BusquedasRapidas();
        Actualizaciones();
        Eliminaciones();
        Recorridos();
        MetricasFinales();
    }

    // 1. Construcción del árbol
    private void ConstruirArbol()
    {
        _vista.MostrarTitulo("1. CONSTRUCCIÓN DEL ÁRBOL");

        foreach (var (nombre, esCarpeta) in DatosIniciales)
            _vista.MostrarInsercion(nombre, esCarpeta, _arbol.Insertar(nombre, esCarpeta));

        _vista.MostrarSubtitulo("Pruebas de validación (duplicados y entradas inválidas)");
        _vista.MostrarInsercion("documentos", true, _arbol.Insertar("documentos", true));
        _vista.MostrarInsercion("ACTAS.PDF", false, _arbol.Insertar("ACTAS.PDF", false));
        _vista.MostrarInsercion("   ", false, _arbol.Insertar("   ", false));
        _vista.MostrarInsercion("(null)", false, _arbol.Insertar(null!, false));

        _vista.MostrarSubtitulo("Árbol inicial");
        _vista.MostrarArbol(_arbol.Raiz);
        _vista.MostrarVerificacionArchivos(_arbol.ArchivosSonHoja());
    }

    // 2. Búsquedas: 2 izquierda, 2 derecha, 2 inexistentes
    private void BusquedasRapidas()
    {
        _vista.MostrarTitulo("2. BÚSQUEDAS RÁPIDAS");

        string[] nombres =
        {
            "Actas.pdf", "Clientes",       // subárbol izquierdo
            "Nomina.xlsx", "Fotos.png",    // subárbol derecho
            "Archivo.log", "Zzz.tmp"       // inexistentes
        };

        foreach (var nombre in nombres)
            _vista.MostrarBusqueda(nombre, _arbol.Buscar(nombre));
    }

    // 3. Actualizaciones: hoja, un hijo, raíz
    private void Actualizaciones()
    {
        _vista.MostrarTitulo("3. ACTUALIZACIONES SELECTIVAS");

        EjecutarActualizacion("Actualizar en HOJA", "Zonas.txt", "Vacaciones.txt");
        EjecutarActualizacion("Actualizar en NODO CON UN HIJO", "Contratos", "Convenios");
        EjecutarActualizacion("Actualizar en la RAÍZ", _arbol.Raiz!.Nombre, "RaizDocumentos");

        _vista.MostrarSubtitulo("Actualización inválida (el nuevo nombre ya existe)");
        _vista.MostrarActualizacion(_arbol.Actualizar("Backups", "informes"));
    }

    private void EjecutarActualizacion(string titulo, string antiguo, string nuevo)
    {
        _vista.MostrarSubtitulo(titulo);
        var objetivo = _arbol.Buscar(antiguo);
        if (objetivo.Encontrado) _vista.MostrarTipoDeNodo(antiguo, objetivo.Nodo!);

        _vista.MostrarActualizacion(_arbol.Actualizar(antiguo, nuevo));
        _vista.MostrarArbol(_arbol.Raiz);
        _vista.MostrarVerificacionArchivos(_arbol.ArchivosSonHoja());
    }

    // 4. Eliminaciones: hoja, un hijo, raíz
    private void Eliminaciones()
    {
        _vista.MostrarTitulo("4. ELIMINACIONES SELECTIVAS");

        EjecutarEliminacion("Eliminar HOJA", "Actas.pdf");
        EjecutarEliminacion("Eliminar NODO CON UN HIJO", "Clientes");
        EjecutarEliminacion("Eliminar RAÍZ (dos hijos, se usa el sucesor inorden)", _arbol.Raiz!.Nombre);

        _vista.MostrarSubtitulo("Eliminación de un nombre inexistente");
        _vista.MostrarEliminacion(_arbol.Eliminar("NoExiste.doc"));
    }

    private void EjecutarEliminacion(string titulo, string nombre)
    {
        _vista.MostrarSubtitulo(titulo);
        var objetivo = _arbol.Buscar(nombre);
        if (objetivo.Encontrado) _vista.MostrarTipoDeNodo(nombre, objetivo.Nodo!);

        _vista.MostrarEliminacion(_arbol.Eliminar(nombre));
        _vista.MostrarArbol(_arbol.Raiz);
        _vista.MostrarVerificacionArchivos(_arbol.ArchivosSonHoja());
    }

    // 5. Recorridos de verificación
    private void Recorridos()
    {
        _vista.MostrarTitulo("5. RECORRIDOS DE VERIFICACIÓN");

        _vista.MostrarRecorrido("Preorden", _arbol.Preorden());
        _vista.MostrarRecorrido("Inorden", _arbol.Inorden());
        _vista.MostrarRecorrido("Postorden", _arbol.Postorden());
        _vista.MostrarRecorrido("Por niveles", _arbol.PorNiveles());
        _vista.MostrarVerificacionInorden(_arbol.InordenOrdenado());
    }

    // 6. Altura final
    private void MetricasFinales()
    {
        _vista.MostrarTitulo("6. ÁRBOL FINAL Y MÉTRICAS");
        _vista.MostrarArbol(_arbol.Raiz);
        _vista.MostrarMetricas(_arbol.Contar(), _arbol.Altura());
    }
}
