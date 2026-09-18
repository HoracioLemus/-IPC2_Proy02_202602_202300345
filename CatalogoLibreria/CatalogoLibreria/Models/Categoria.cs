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

        if (primeraSubcategoria == null || string.Compare(sub.Nombre, primeraSubcategoria.Dato.Nombre) < 0)
        {
            nuevo.Siguiente = primeraSubcategoria;
            primeraSubcategoria = nuevo;
            return;
        }
        
        //buscar posicion correcta recorriendo la lista
        NodoCategoria actual = primeraSubcategoria;
        while (actual.Siguiente != null && string.Compare(actual.Siguiente.Dato.Nombre, sub.Nombre) < 0)
            actual = actual.Siguiente;

        nuevo.Siguiente = actual.Siguiente;
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
        while (actual.Siguiente != null)
            actual = actual.Siguiente;
        actual.Siguiente = nuevo;
    }

    public NodoCategoria ObtenerSubcategorias() => primeraSubcategoria;
    public NodoLibroEnCategoria ObtenerLibros() => primerLibro;
    
    //Busqueda por Categoria
    public Categoria BuscarCategoria(string nombre)
    {
        if (Nombre == nombre)
            return this;

        NodoCategoria actual = primeraSubcategoria;
        while (actual != null)
        {
            Categoria resultado = actual.Dato.BuscarCategoria(nombre);
            if (resultado != null)
                return resultado;
            actual = actual.Siguiente;
        }
        return null;
    }
    
    //Mostrar en Jerarquia
    public void MostrarJerarquia(int nivel = 0)
    {
        string sangria = new string('-', nivel * 2);
        Console.WriteLine($"{sangria}{Nombre}");
        
        //Mostrar Categoria
        NodoLibroEnCategoria nodoLibro = primerLibro;
        while (nodoLibro != null)
        {
            Console.WriteLine($"{sangria} * {nodoLibro.Dato.Titulo} (ISBN: {nodoLibro.Dato.ISBN})");
            nodoLibro = nodoLibro.Siguiente;
        }
        
        //Mostrar subcategoria
        NodoCategoria nodoSub = primeraSubcategoria;
        while (nodoSub != null)
        {
            nodoSub.Dato.MostrarJerarquia(nivel + 1);
            nodoSub = nodoSub.Siguiente;
        }
    }
}