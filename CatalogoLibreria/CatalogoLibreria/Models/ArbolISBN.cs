namespace CatalogoLibreria.Models;
using System.Text;
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
    
    //Recorrido in-order
    public void MostrarAscendente()
    {
        MostrarAscendenteRecursivo(raiz);
    }

    private void MostrarAscendenteRecursivo(NodoISBN nodo)
    {
        if (nodo == null)
            return;
        
         
        MostrarAscendenteRecursivo(nodo.Izquierda);
        Console.WriteLine($"ISBN: {nodo.Dato.ISBN} - {nodo.Dato.Titulo}");
        MostrarAscendenteRecursivo(nodo.Derecha);
    }
    
    //Eliminar del BST
    public void Eliminar(int isbn)
    {
        raiz = EliminarRecursivo(raiz, isbn);
    }

    private NodoISBN EliminarRecursivo(NodoISBN nodo, int isbn)
    {
        if (nodo == null)
            return null;

        if (isbn < nodo.Dato.ISBN)
            nodo.Izquierda = EliminarRecursivo(nodo.Izquierda, isbn);
        else if (isbn > nodo.Dato.ISBN)
            nodo.Derecha = EliminarRecursivo(nodo.Derecha, isbn);
        else
        {
            // Nodo a eliminar encontrado
           //sin hijos
           if (nodo.Izquierda == null && nodo.Derecha == null)
               return null;
           
           //un solo hijo
           if (nodo.Izquierda != null)
               return nodo.Derecha;
           if (nodo.Derecha == null)
               return nodo.Izquierda;
           
           //dos hijos
           NodoISBN sucesor = nodo.Derecha;
           while (sucesor.Izquierda != null)
               sucesor = sucesor.Izquierda;
           
           //Reemplazo y eliminar sucesor de posicion original
           nodo.Dato = sucesor.Dato;
           nodo.Derecha = EliminarRecursivo(nodo.Derecha, sucesor.Dato.ISBN);
        }

        return nodo;
    }

    private NodoISBN ObtenerSucesor(NodoISBN nodo)
    {
        while (nodo.Izquierda != null)
            nodo = nodo.Izquierda;
        return nodo;
    }
    
    //Texto para .dot
    public string ObtenerNodosDot()
    {
        StringBuilder sb = new StringBuilder();
        ObtenerNodosDotRecursivo(raiz, sb);
        return sb.ToString();
    }

    private void ObtenerNodosDotRecursivo(NodoISBN nodo, StringBuilder sb)
    {
        if (nodo == null)
        {
            return;
        }

        ObtenerNodosDotRecursivo(nodo.Izquierda, sb);
        sb.AppendLine($" \"{nodo.Dato.ISBN}\" [label=\"{nodo.Dato.Titulo}\\nISBN: {nodo.Dato.ISBN}\"];");
        ObtenerNodosDotRecursivo(nodo.Derecha, sb);
    }
}