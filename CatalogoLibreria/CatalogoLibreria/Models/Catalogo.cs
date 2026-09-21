using System.Text;

namespace CatalogoLibreria.Models;
using System.Xml.Linq;

//Catalogo
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
            if (padre!= null)
                padre.AgregarSubcategoria(nueva);
            else
            {
                Console.WriteLine($"Categoria padre '{nombrePadre}' no encontrada.");
            }
            
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
        public void EliminarLibro(int isbn) => arbolLibros.Eliminar(isbn);
        public void MostrarLibrosAscendente() => arbolLibros.MostrarAscendente();
        
        public void CargarXml(string rutaArchivo)
        {
            XDocument doc = XDocument.Load(rutaArchivo);

            XElement listaCategorias = doc.Root.Element("listaCategorias");
            if (listaCategorias != null)
            {
                foreach (XElement categoriaElem in listaCategorias.Elements("categoria"))
                {
                    string nombre = categoriaElem.Value.Trim();
                    string padre = categoriaElem.Attribute("padre")?.Value;
                    AgregarCategoria(nombre, padre);
                }
            }

            XElement listaLibros = doc.Root.Element("listaLibros");
            if (listaLibros != null)
            {
                foreach (XElement libroElem in listaLibros.Elements("libro"))
                {
                    int isbn = int.Parse(libroElem.Element("ISBN").Value);
                    string titulo = libroElem.Element("titulo").Value;
                    string autor = libroElem.Element("autor").Value;
                    string categoria = libroElem.Element("categoria").Value;

                    RegistrarLibro(isbn, titulo, autor, categoria);
                }
            }
        }
        
        //Mostrar HTML
        public string GenerarHtmlJerarquiaCompleta()
        {
            StringBuilder sb = new StringBuilder();
            NodoCategoria actual = raizCategorias.ObtenerSubcategorias();
            while (actual !=null)
            {
                sb.Append(actual.Dato.GenerarHtmlJerarquia());
                actual = actual.Siguiente;
            }

            return sb.ToString();
        }
    }