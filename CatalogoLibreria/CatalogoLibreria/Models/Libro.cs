namespace CatalogoLibreria.Models;

public class Libro
{
    public int ISBN;
    public string Titulo;
    public string Autor;
    public Categoria CategoriaAsociada;
    
    public Libro (int isbn, string titulo, string autor, Categoria categoria)
    {
        ISBN = isbn;
        Titulo = titulo;
        Autor = autor;
        CategoriaAsociada = categoria;
    }
}