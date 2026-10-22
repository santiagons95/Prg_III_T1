namespace DocuTrack.Model;

/// BST simple ordenado por Nombre, comparación ordinal sin distinguir mayúsculas. Esta clase no tiene ninguna dependencia de la consola.
public class ArbolBinario
{
    public Nodo? Raiz { get; private set; }

    private static int Comparar(string a, string b) =>
        string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

    // ---------------------------------------------------------------- CRUD

    ///Inserta un nodo. Rechaza nombres vacíos y duplicados.
    public ResultadoInsercion Insertar(string nombre, bool esCarpeta)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new ResultadoInsercion(false, 0, "El nombre no puede ser nulo ni vacío.");

        nombre = nombre.Trim();
        var nuevo = new Nodo(nombre, esCarpeta);

        if (Raiz == null)
        {
            Raiz = nuevo;
            return new ResultadoInsercion(true, 0, "Insertado como raíz.");
        }

        int comparaciones = 0;
        Nodo actual = Raiz;
        bool aLaIzquierda;
        while (true)
        {
            comparaciones++;
            int cmp = Comparar(nombre, actual.Nombre);

            if (cmp == 0)
                return new ResultadoInsercion(false, comparaciones,
                    $"Duplicado rechazado: '{nombre}' ya existe como '{actual.Nombre}'.");

            aLaIzquierda = cmp < 0;
            Nodo? siguiente = aLaIzquierda ? actual.Izquierdo : actual.Derecho;
            if (siguiente == null)
            {
                if (aLaIzquierda) actual.Izquierdo = nuevo; else actual.Derecho = nuevo;
                break;
            }
            actual = siguiente;
        }

        return new ResultadoInsercion(true, comparaciones,
            $"Insertado como hijo {(aLaIzquierda ? "izquierdo" : "derecho")} de '{actual.Nombre}'.");
    }

    /// Busca un nombre y cuenta las comparaciones realizadas.
    public ResultadoBusqueda Buscar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new ResultadoBusqueda(false, null, 0);

        int comparaciones = 0;
        Nodo? actual = Raiz;
        while (actual != null)
        {
            comparaciones++;
            int cmp = Comparar(nombre.Trim(), actual.Nombre);
            if (cmp == 0) return new ResultadoBusqueda(true, actual, comparaciones);
            actual = cmp < 0 ? actual.Izquierdo : actual.Derecho;
        }
        return new ResultadoBusqueda(false, null, comparaciones);
    }

    /// Actualiza el nombre como Eliminar(antiguo) + Insertar(nuevo), conservando EsCarpeta.
    /// Valida antes de eliminar para no perder el nodo si la operación no es válida.
    public ResultadoActualizacion Actualizar(string nombreAntiguo, string nombreNuevo)
    {
        if (string.IsNullOrWhiteSpace(nombreAntiguo) || string.IsNullOrWhiteSpace(nombreNuevo))
            return new ResultadoActualizacion(false, "Los nombres no pueden ser nulos ni vacíos.", null, null);

        var existente = Buscar(nombreAntiguo);
        if (!existente.Encontrado)
            return new ResultadoActualizacion(false, $"'{nombreAntiguo}' no existe.", null, null);

        if (Buscar(nombreNuevo).Encontrado)
            return new ResultadoActualizacion(false,
                $"El nuevo nombre '{nombreNuevo}' ya existe; actualización rechazada.", null, null);

        bool esCarpeta = existente.Nodo!.EsCarpeta;
        var eliminacion = Eliminar(nombreAntiguo);
        var insercion = Insertar(nombreNuevo, esCarpeta);

        return new ResultadoActualizacion(eliminacion.Exito && insercion.Exito,
            $"'{nombreAntiguo}' -> '{nombreNuevo.Trim()}'", eliminacion, insercion);
    }

    ///Elimina un nombre. En el caso de dos hijos usa el sucesor inorden.
    public ResultadoEliminacion Eliminar(string nombre)
    {
        var resultado = new ResultadoEliminacion();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            resultado.Mensaje = "El nombre no puede ser nulo ni vacío.";
            return resultado;
        }

        Raiz = EliminarRecursivo(Raiz, nombre.Trim(), resultado);
        resultado.Mensaje = resultado.Exito
            ? $"Eliminado '{nombre.Trim()}'."
            : $"'{nombre.Trim()}' no existe.";
        return resultado;
    }

    private Nodo? EliminarRecursivo(Nodo? actual, string nombre, ResultadoEliminacion res)
    {
        if (actual == null) return null;

        res.Comparaciones++;
        int cmp = Comparar(nombre, actual.Nombre);

        if (cmp < 0)
        {
            actual.Izquierdo = EliminarRecursivo(actual.Izquierdo, nombre, res);
            return actual;
        }
        if (cmp > 0)
        {
            actual.Derecho = EliminarRecursivo(actual.Derecho, nombre, res);
            return actual;
        }

        res.Exito = true;

        if (actual.EsHoja)
        {
            res.Caso = CasoEliminacion.Hoja;
            return null;
        }
        if (actual.Izquierdo == null)
        {
            res.Caso = CasoEliminacion.UnHijo;
            return actual.Derecho;
        }
        if (actual.Derecho == null)
        {
            res.Caso = CasoEliminacion.UnHijo;
            return actual.Izquierdo;
        }

        // Dos hijos: el sucesor (mínimo del subárbol derecho) ocupa el lugar del nodo.
        Nodo sucesor = Minimo(actual.Derecho);
        res.Caso = CasoEliminacion.DosHijos;
        res.NombreSucesor = sucesor.Nombre;

        actual.Nombre = sucesor.Nombre;
        actual.EsCarpeta = sucesor.EsCarpeta;

        var interno = new ResultadoEliminacion();
        actual.Derecho = EliminarRecursivo(actual.Derecho, sucesor.Nombre, interno);
        res.Comparaciones += interno.Comparaciones;
        return actual;
    }

    private static Nodo Minimo(Nodo nodo)
    {
        while (nodo.Izquierdo != null) nodo = nodo.Izquierdo;
        return nodo;
    }

    // ----------------------------------------------------------- Recorridos

    public List<Nodo> Preorden()
    {
        var lista = new List<Nodo>();
        void Visitar(Nodo? n)
        {
            if (n == null) return;
            lista.Add(n);
            Visitar(n.Izquierdo);
            Visitar(n.Derecho);
        }
        Visitar(Raiz);
        return lista;
    }

    public List<Nodo> Inorden()
    {
        var lista = new List<Nodo>();
        void Visitar(Nodo? n)
        {
            if (n == null) return;
            Visitar(n.Izquierdo);
            lista.Add(n);
            Visitar(n.Derecho);
        }
        Visitar(Raiz);
        return lista;
    }

    public List<Nodo> Postorden()
    {
        var lista = new List<Nodo>();
        void Visitar(Nodo? n)
        {
            if (n == null) return;
            Visitar(n.Izquierdo);
            Visitar(n.Derecho);
            lista.Add(n);
        }
        Visitar(Raiz);
        return lista;
    }

    ///Recorrido por niveles (BFS) usando una cola.
    public List<Nodo> PorNiveles()
    {
        var lista = new List<Nodo>();
        if (Raiz == null) return lista;

        var cola = new Queue<Nodo>();
        cola.Enqueue(Raiz);
        while (cola.Count > 0)
        {
            Nodo actual = cola.Dequeue();
            lista.Add(actual);
            if (actual.Izquierdo != null) cola.Enqueue(actual.Izquierdo);
            if (actual.Derecho != null) cola.Enqueue(actual.Derecho);
        }
        return lista;
    }

    // ------------------------------------------------------------ Auxiliares

    /// Altura en niveles: árbol vacío = 0, solo raíz = 1.
    public int Altura() => Altura(Raiz);

    private static int Altura(Nodo? nodo) =>
        nodo == null ? 0 : 1 + Math.Max(Altura(nodo.Izquierdo), Altura(nodo.Derecho));

    public int Contar() => Inorden().Count;

    /// Verifica que el inorden esté estrictamente ascendente (propiedad BST).
    public bool InordenOrdenado()
    {
        var nodos = Inorden();
        for (int i = 1; i < nodos.Count; i++)
            if (Comparar(nodos[i - 1].Nombre, nodos[i].Nombre) >= 0) return false;
        return true;
    }

    /// Verifica la regla de dominio: todo archivo es hoja.
    public bool ArchivosSonHoja() =>
        Preorden().All(n => n.EsCarpeta || n.EsHoja);
}
