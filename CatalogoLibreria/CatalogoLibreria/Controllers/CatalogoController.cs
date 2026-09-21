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
}