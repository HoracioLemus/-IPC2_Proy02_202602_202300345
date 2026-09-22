using System.Text;
using System.Linq;
namespace CatalogoLibreria.Models;
using System.Xml.Linq;

public class Catalogo
{
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
        var elementos = listaCategorias.Elements("categoria").ToArray();
        int cantidad = elementos.Length;

        Categoria[] nuevas = new Categoria[cantidad];
        string[] padres = new string[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            string nombre = elementos[i].Value.Trim();
            string padre = elementos[i].Attribute("padre")?.Value;

            bool yaExiste = BuscarCategoria(nombre) != null;
            for (int j = 0; j < i && !yaExiste; j++)
            {
                if (nuevas[j] != null && nuevas[j].Nombre == nombre)
                    yaExiste = true;
            }

            if (yaExiste)
            {
                Console.WriteLine($"Categoria '{nombre}' ya existe, se omite.");
                continue;
            }

            nuevas[i] = new Categoria(nombre);
            padres[i] = padre;
        }

        for (int i = 0; i < cantidad; i++)
        {
            if (nuevas[i] == null)
                continue;

            if (string.IsNullOrEmpty(padres[i]))
            {
                raizCategorias.AgregarSubcategoria(nuevas[i]);
                continue;
            }

            Categoria padreCat = BuscarCategoria(padres[i]);

            if (padreCat == null)
            {
                for (int j = 0; j < cantidad; j++)
                {
                    if (nuevas[j] != null && nuevas[j].Nombre == padres[i])
                    {
                        padreCat = nuevas[j];
                        break;
                    }
                }
            }

            if (padreCat != null)
                padreCat.AgregarSubcategoria(nuevas[i]);
            else
                Console.WriteLine($"Categoria padre '{padres[i]}' no encontrada.");
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