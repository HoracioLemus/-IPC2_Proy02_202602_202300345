namespace CatalogoLibreria.Models;
//Nodo para subcategorias
public class NodoCategoria
{
    public Categoria Dato;
    public NodoCategoria Siguiente;
    
    public NodoCategoria (Categoria dato)
    {
        Dato = dato;
        Siguiente = null;
    }
}
//Nodo para enlazar libros en una categoria
public class NodoLibroEnCategoria
{
    public Libro Dato;
    public NodoLibroEnCategoria Siguiente;

    public NodoLibroEnCategoria(Libro dato)
    {
        Dato = dato;
        Siguiente = null;
    }

    
}
public class Categoria
{
    public string Nombre;
    private NodoCategoria primeraSubcategoria;
    private NodoLibroEnCategoria primerLibro;

    public Categoria(string nombre)
    {
        Nombre = nombre;
        primeraSubcategoria = null;
        primerLibro = null;
    }

    public void AgregarSubcategoria(Categoria sub)
    {
        NodoCategoria nuevo = new NodoCategoria(sub);
        if (primeraSubcategoria == null) 
        {
            primeraSubcategoria = nuevo;
            return;
        }

        NodoCategoria actual = primeraSubcategoria;
        while (actual != null)
            actual = actual.Siguiente;
        actual.Siguiente = nuevo;
    }
    
    //Asocia un libro existente a la categoria
    public void AgregarLibro(Libro libro)
    {
        NodoLibroEnCategoria nuevo = new NodoLibroEnCategoria(libro);
        if (primerLibro == null)
        {
            primerLibro = nuevo;
            return;
        }

        NodoLibroEnCategoria actual = primerLibro;
        while (actual != null)
            actual = actual.Siguiente;
        actual.Siguiente = nuevo;
    }

    public NodoCategoria ObtenerSubcategorias() => primeraSubcategoria;
    public NodoLibroEnCategoria ObtenerLibros() => primerLibro;
}