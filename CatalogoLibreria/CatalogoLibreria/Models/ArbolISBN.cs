namespace CatalogoLibreria.Models;

public class NodoISBN
{
    public Libro Dato;
    public NodoISBN Izquierda;
    public NodoISBN Derecha;

    public NodoISBN(Libro dato)
    {
        Dato = dato;
        Izquierda = null;
        Derecha = null;
    }
}
public class ArbolISBN
{
    private NodoISBN raiz;

    public ArbolISBN()
    {
        raiz = null;
    }

    public void Insertar(Libro libro)
    {
        raiz = InsertarRecursivo(raiz, libro);
    }

    private NodoISBN InsertarRecursivo(NodoISBN nodo, Libro libro)
    {
        if (nodo == null)
            return new NodoISBN(libro);
        if (libro.ISBN < nodo.Dato.ISBN)
            nodo.Izquierda = InsertarRecursivo(nodo.Izquierda, libro);
        else if (libro.ISBN > nodo.Dato.ISBN)
            nodo.Derecha = InsertarRecursivo(nodo.Derecha, libro);

        return nodo;
    }

    public Libro Buscar(int isbn)
    {
        return BuscarRecursivo(raiz, isbn);
    }

    private Libro BuscarRecursivo(NodoISBN nodo, int isbn){
        if (nodo==null)
        {
            return null;
        }

        if (isbn == nodo.Dato.ISBN)
        {
            return nodo.Dato;
        }

        if (isbn < nodo.Dato.ISBN)
        {
            return BuscarRecursivo(nodo.Izquierda, isbn);
        }
        else
        {
            return BuscarRecursivo(nodo.Derecha, isbn);
        }
    }

    public Libro ObtenerMinimo()
    {
        if (raiz == null)
            return null;

        NodoISBN actual = raiz;
        while (actual.Izquierda != null)
            actual = actual.Izquierda;
        return actual.Dato;
    }

    public Libro ObtenerMaximo()
    {
        if (raiz == null)
            return null;

        NodoISBN actual = raiz;
        while (actual.Derecha != null)
            actual = actual.Derecha;
        return actual.Dato;
    }
}