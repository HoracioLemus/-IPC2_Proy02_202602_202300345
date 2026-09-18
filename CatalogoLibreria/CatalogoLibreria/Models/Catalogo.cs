namespace CatalogoLibreria.Models;

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
    }