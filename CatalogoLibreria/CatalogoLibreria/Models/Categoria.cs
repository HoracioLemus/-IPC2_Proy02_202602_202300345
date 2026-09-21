using System.Text;
using System.Diagnostics;
using System.IO;

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

    //Generar Dot
    public string GenerarDot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("digraph Categoria {");
        sb.AppendLine("    node [shape=box];");

        //Obtener Libros de la categotia en orden ascendente de ISBN
        Libro[] libroOrdenados = ObtenerLibrosOrdenados();

        foreach (Libro libro in ObtenerLibrosOrdenados())
        {
            sb.AppendLine($"     \"{libro.ISBN}\" [label=\"{libro.Titulo}\\nISBN: {libro.ISBN}\"];");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    //copia de los libros en la lista enlazada a un arreglo ordenado por ISBN
    private Libro[] ObtenerLibrosOrdenados()
    {
        int cantidad = 0;
        NodoLibroEnCategoria nodo = primerLibro;
        while (nodo != null)
        {
            cantidad++;
            nodo = nodo.Siguiente;
        }

        Libro[] libros = new Libro[cantidad];
        nodo = primerLibro;
        int i = 0;
        while (nodo != null)
        {
            libros[i] = nodo.Dato;
            i++;
            nodo = nodo.Siguiente;
        }

        //Ordenar por ISBN
        for (int a = 0; a < libros.Length - 1; a++)
        {
            for (int b = 0; b < libros.Length - 1 - a; b++)
            {
                if (libros[b].ISBN > libros[b + 1].ISBN)
                {
                    (libros[b], libros[b + 1]) = (libros[b + 1], libros[b]);
                }
            }
        }

        return libros;
    }

    public string GenerarImagen(string rutaSalida)
    {
        string textoDot = GenerarDot();

        string rutaDot = Path.ChangeExtension(rutaSalida, ".dot");
        File.WriteAllText(rutaDot, textoDot);

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = "dot",
            Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaSalida}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process proceso = Process.Start(info))
        {
            string salidaError = proceso.StandardError.ReadToEnd();
            string salidaNormal = proceso.StandardOutput.ReadToEnd();
            proceso.WaitForExit();

            Console.WriteLine($"Codigo de salida de dot: {proceso.ExitCode}");
            if (!string.IsNullOrWhiteSpace(salidaError))
                Console.WriteLine($"Error de dot: {salidaError}");
            if (!string.IsNullOrWhiteSpace(salidaNormal))
                Console.WriteLine($"Salida de dot: {salidaNormal}");
        }

        return rutaSalida;
    }
    
    //Mostrar en HTML
    public string GenerarHtmlJerarquia(int nivel = 0)
    {
        StringBuilder sb = new StringBuilder();
        string sangria = new string('-', nivel * 2);

        sb.AppendLine($"<p style='margin-left:{nivel * 20}px'><strong>{sangria}{Nombre}</strong></p>");

        NodoLibroEnCategoria nodoLibro = primerLibro;
        while (nodoLibro != null)
        {
            sb.AppendLine($"<p style='margin-left:{(nivel + 1) * 20}px'>* {nodoLibro.Dato.Titulo} (ISBN: {nodoLibro.Dato.ISBN})</p>");
            nodoLibro = nodoLibro.Siguiente;
        }

        NodoCategoria nodoSub = primeraSubcategoria;
        while (nodoSub != null)
        {
            sb.Append(nodoSub.Dato.GenerarHtmlJerarquia(nivel + 1));
            nodoSub = nodoSub.Siguiente;
        }

        return sb.ToString();
    }

}