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
        return View();
    }
}