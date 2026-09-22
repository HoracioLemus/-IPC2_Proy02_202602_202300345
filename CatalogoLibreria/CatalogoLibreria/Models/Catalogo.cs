using System.Text;
namespace CatalogoLibreria.Models;
using System.Xml.Linq;

public class Catalogo
{
    private class NodoCategoriaPendiente
    {
        public string Nombre;
        public string Padre;
        public NodoCategoriaPendiente Siguiente;
        public Categoria CategoriaCreada;

        public NodoCategoriaPendiente(string nombre, string padre)
        {
            Nombre = nombre;
            Padre = padre;
            Siguiente = null;
            CategoriaCreada = null;
        }
    }

    private Categoria raizCategorias;
    private ArbolISBN arbolLibros;

    public Catalogo()
    {
        raizCategorias = new Categoria("__raiz__");
        arbolLibros = new ArbolISBN();
    }

    //--Categorias--
    public void AgregarCategoria(string nombre, string nombrePadre)
    {
        if (BuscarCategoria(nombre) != null)
        {
            Console.WriteLine($"Categoria '{nombre}' ya existe, se omite.");
            return;
        }

        Categoria nueva = new Categoria(nombre);

        if (string.IsNullOrEmpty(nombrePadre))
        {
            raizCategorias.AgregarSubcategoria(nueva);
            return;
        }

        Categoria padre = raizCategorias.BuscarCategoria(nombrePadre);
        if (padre != null)
            padre.AgregarSubcategoria(nueva);
        else
            Console.WriteLine($"Categoria padre '{nombrePadre}' no encontrada.");
    }

    public Categoria BuscarCategoria(string nombre)
    {
        return raizCategorias.BuscarCategoria(nombre);
    }

    public void MostrarJerarquiaCompleta()
    {
        NodoCategoria actual = raizCategorias.ObtenerSubcategorias();
        while (actual != null)
        {
            actual.Dato.MostrarJerarquia();
            actual = actual.Siguiente;
        }
    }

    // Carga de categorias con linking diferido: primero crea todas, luego las conecta con su padre
    private void CargarCategoriasDiferido(XElement listaCategorias)
    {
        NodoCategoriaPendiente primeraPendiente = null;
        NodoCategoriaPendiente ultimaPendiente = null;

        foreach (XElement elemento in listaCategorias.Elements("categoria"))
        {
            string nombre = elemento.Value.Trim();
            string padre = elemento.Attribute("padre")?.Value;
            NodoCategoriaPendiente nuevo = new NodoCategoriaPendiente(nombre, padre);

            if (primeraPendiente == null)
            {
                primeraPendiente = nuevo;
                ultimaPendiente = nuevo;
            }
            else
            {
                ultimaPendiente.Siguiente = nuevo;
                ultimaPendiente = nuevo;
            }
        }

        NodoCategoriaPendiente actual = primeraPendiente;
        while (actual != null)
        {
            bool yaExiste = BuscarCategoria(actual.Nombre) != null;
            NodoCategoriaPendiente anterior = primeraPendiente;

            while (anterior != actual && !yaExiste)
            {
                if (anterior.CategoriaCreada != null &&
                    anterior.CategoriaCreada.Nombre == actual.Nombre)
                {
                    yaExiste = true;
                }

                anterior = anterior.Siguiente;
            }

            if (yaExiste)
            {
                Console.WriteLine($"Categoria '{actual.Nombre}' ya existe, se omite.");
            }
            else
            {
                actual.CategoriaCreada = new Categoria(actual.Nombre);
            }

            actual = actual.Siguiente;
        }

        actual = primeraPendiente;
        while (actual != null)
        {
            if (actual.CategoriaCreada == null)
            {
                actual = actual.Siguiente;
                continue;
            }

            if (string.IsNullOrEmpty(actual.Padre))
            {
                raizCategorias.AgregarSubcategoria(actual.CategoriaCreada);
                actual = actual.Siguiente;
                continue;
            }

            Categoria padreCat = BuscarCategoria(actual.Padre);
            NodoCategoriaPendiente posiblePadre = primeraPendiente;

            while (padreCat == null && posiblePadre != null)
            {
                if (posiblePadre.CategoriaCreada != null &&
                    posiblePadre.CategoriaCreada.Nombre == actual.Padre)
                {
                    padreCat = posiblePadre.CategoriaCreada;
                }

                posiblePadre = posiblePadre.Siguiente;
            }

            if (padreCat != null)
                padreCat.AgregarSubcategoria(actual.CategoriaCreada);
            else
                Console.WriteLine($"Categoria padre '{actual.Padre}' no encontrada.");

            actual = actual.Siguiente;
        }
    }

    //--Libros--
    public void RegistrarLibro(int isbn, string titulo, string autor, string nombreCategoria)
    {
        if (arbolLibros.Buscar(isbn) != null)
        {
            Console.WriteLine($"Libro con ISBN {isbn} ya existe, se omite.");
            return;
        }

        Categoria categoria = BuscarCategoria(nombreCategoria);
        if (categoria == null)
        {
            Console.WriteLine($"Categoria '{nombreCategoria}' no encontrada.");
            return;
        }

        Libro libro = new Libro(isbn, titulo, autor, categoria);
        categoria.AgregarLibro(libro);
        arbolLibros.Insertar(libro);
    }

    public Libro BuscarLibro(int isbn) => arbolLibros.Buscar(isbn);
    public Libro ObtenerLibroMinimo() => arbolLibros.ObtenerMinimo();
    public Libro ObtenerLibroMaximo() => arbolLibros.ObtenerMaximo();

    public void EliminarLibro(int isbn)
    {
        Libro libro = arbolLibros.Buscar(isbn);
        if (libro == null)
            return;

        libro.CategoriaAsociada.EliminarLibro(isbn);
        arbolLibros.Eliminar(isbn);
    }

    public void MostrarLibrosAscendente() => arbolLibros.MostrarAscendente();

    public void CargarXml(string rutaArchivo)
    {
        XDocument doc = XDocument.Load(rutaArchivo);

        XElement listaCategorias = doc.Root.Element("listaCategorias");
        if (listaCategorias != null)
        {
            CargarCategoriasDiferido(listaCategorias);
        }

        XElement listaLibros = doc.Root.Element("listaLibros");
        if (listaLibros != null)
        {
            foreach (XElement libroElem in listaLibros.Elements("libro"))
            {
                int isbn;
                bool esValido = int.TryParse(libroElem.Element("ISBN")?.Value, out isbn);

                if (!esValido)
                {
                    Console.WriteLine("Libro con ISBN no numerico o invalido, se omite.");
                    continue;
                }

                string titulo = libroElem.Element("titulo")?.Value;
                string autor = libroElem.Element("autor")?.Value;
                string categoria = libroElem.Element("categoria")?.Value;

                RegistrarLibro(isbn, titulo, autor, categoria);
            }
        }
    }

    public string GenerarHtmlJerarquiaCompleta()
    {
        StringBuilder sb = new StringBuilder();
        NodoCategoria actual = raizCategorias.ObtenerSubcategorias();
        while (actual != null)
        {
            sb.Append(actual.Dato.GenerarHtmlJerarquia());
            actual = actual.Siguiente;
        }

        return sb.ToString();
    }
}