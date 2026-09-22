namespace CatalogoLibreria.Controllers;
using Microsoft.AspNetCore.Mvc;
using CatalogoLibreria.Models;

public class CatalogoController : Controller
{
    private readonly Catalogo _catalogo;

    public CatalogoController(Catalogo catalogo)
    {
        _catalogo = catalogo;
    }

    public IActionResult Index()
    {
       

        ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
        return View();
    }

    [HttpPost]
    public IActionResult RegistrarLibro(int isbn, string titulo, string autor, string categoria)
    {
        _catalogo.RegistrarLibro(isbn, titulo, autor, categoria);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult GenerarGrafico(string categoria)
    {
        Categoria cat = _catalogo.BuscarCategoria(categoria);
        if (cat == null)
        {
            ViewBag.ErrorGrafico = $"Categoria '{categoria}' no encontrada.";
            ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
            return View("Index");
        }

        Directory.CreateDirectory("wwwroot/graphs");

        string nombreArchivo = categoria.Replace(" ", "_") + ".png";
        string rutaCompleta = "wwwroot/graphs/" + nombreArchivo;
        cat.GenerarImagen(rutaCompleta);

        ViewBag.RutaImagen = "/graphs/" + nombreArchivo + "?t=" + DateTime.Now.Ticks;
        ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
        return View("Index");
    }

    [HttpPost]
    public IActionResult BuscarPorIsbn(int isbn)
    {
        Libro libro = _catalogo.BuscarLibro(isbn);
        ViewBag.LibroEncontrado = libro;
        ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
        return View("Index");
    }

    public IActionResult VerMinMax()
    {
        ViewBag.LibroMin = _catalogo.ObtenerLibroMinimo();
        ViewBag.LibroMax = _catalogo.ObtenerLibroMaximo();
        ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
        return View("Index");
    }

    [HttpPost]
    public IActionResult EliminarLibro(int isbn)
    {
        _catalogo.EliminarLibro(isbn);
        return RedirectToAction("Index");
    }

    public IActionResult Ayuda()
    {
        return View();
    }
    
    [HttpPost]
        public IActionResult CargarXmlDesdeArchivo(IFormFile archivoXml){
            if (archivoXml == null || archivoXml.Length == 0)
            {
                ViewBag.ErrorCarga = "No se selecciono ningun archivo.";
                ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
                return View("Index");
            }

            string rutaTemporal = Path.Combine("Data", "subido_" + Guid.NewGuid() + ".xml");

            using (var stream = new FileStream(rutaTemporal, FileMode.Create))
            {
                archivoXml.CopyTo(stream);
            }

            _catalogo.CargarXml(rutaTemporal);

            ViewBag.JerarquiaHtml = _catalogo.GenerarHtmlJerarquiaCompleta();
            return View("Index");
        }
}