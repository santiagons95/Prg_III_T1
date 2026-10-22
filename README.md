# Programación III · Universidad de Manizales · 2026-2 
# Estudiante: Santiago Noreña
# Primera entrega: Árboles Binarios
# Docente: Carlos Alberto Gutierrez

```bash
git clone <URL-DEL-REPOSITORIO>
cd DocuTrack
dotnet run
```

## Estructura

```
DocuTrack/
├── Program.cs                      # Solo ensambla capas e invoca al Controller
├── Model/
│   ├── Nodo.cs                     # Nombre, EsCarpeta, Izquierdo, Derecho
│   ├── ArbolBinario.cs             # CRUD, recorridos, altura (sin Console)
│   └── Resultados.cs               # Resultados de operaciones (comparaciones, caso)
├── View/
│   └── ConsolaVista.cs             # Toda la presentación, incluido el árbol ASCII
└── Controller/
    └── DocuTrackController.cs      # Orquesta los casos de uso
```

## Decisiones de diseño

- Clave: `Nombre`, comparada con `StringComparison.OrdinalIgnoreCase`.
- Duplicados: se rechazan en `Insertar`; la vista registra el intento (`RECHAZADO`).
- Validaciones: nulos, vacíos y espacios en blanco se rechazan en el Model.
- Actualizar: `Eliminar(antiguo)` + `Insertar(nuevo)`. Antes de eliminar se valida que el
  nombre nuevo no exista, para no perder el nodo si la operación es inválida. Se conserva `EsCarpeta`.
- Eliminar (dos hijos): se usa siempre el sucesor inorden (mínimo del subárbol derecho),
  copiando sus datos al nodo y eliminando después el sucesor. La vista informa el caso ejecutado.
- Archivos siempre hoja: el conjunto de datos coloca carpetas en los nodos internos y el
  Model expone `ArchivosSonHoja()`; el Controller lo verifica tras cada operación.
- Altura: en niveles (árbol vacío = 0, solo raíz = 1).
- Comparaciones: se cuentan en `Buscar`, `Insertar` y `Eliminar` y se muestran en la vista.

## ¿Qué recorrido confirma que el BST sigue ordenado?

El inorden (izquierdo → nodo → derecho). En un BST todo valor del subárbol izquierdo es menor
que el nodo y todo valor del derecho es mayor, por lo que el inorden visita los nombres en orden
ascendente. Si tras insertar, actualizar o eliminar el inorden sigue estrictamente ascendente, la
propiedad del BST se conserva. `ArbolBinario.InordenOrdenado()` lo comprueba y la vista lo reporta.
Preorden, postorden y por niveles dependen de la forma del árbol y no sirven para esa verificación.

## Casos de uso que ejecuta el programa

1. Construcción con 14 nombres + pruebas de duplicados/vacíos + árbol ASCII inicial.
2. 6 búsquedas (2 izquierda, 2 derecha, 2 inexistentes) con comparaciones.
3. 3 actualizaciones (hoja, un hijo, raíz) con árbol tras cada una.
4. 3 eliminaciones (hoja, un hijo, raíz) con árbol tras cada una.
5. Recorridos: preorden, inorden, postorden y por niveles.
6. Árbol final, total de nodos y altura.

## Entrega

Etiquetar el commit final: `git tag release-unidad1 && git push --tags`.
