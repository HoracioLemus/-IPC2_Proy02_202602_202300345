using CatalogoLibreria.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Catalogo>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
/*
// Cargar datos iniciales una sola vez al arrancar
using (var scope = app.Services.CreateScope())
{
    var catalogo = scope.ServiceProvider.GetRequiredService<Catalogo>();
    catalogo.CargarXml("Data/entrada_prueba.xml");
}


//---Prueba temporal --
Console.WriteLine("--- Prueba de carga XML ---");
Catalogo catalogoXml = new Catalogo();
catalogoXml.CargarXml("Data/entrada_prueba.xml");
catalogoXml.MostrarJerarquiaCompleta();

Console.WriteLine("--- Generar .dot de Ficcion ---");
Categoria ficcionEncontrada = catalogoXml.BuscarCategoria("Ficción");
if (ficcionEncontrada != null)
    Console.WriteLine(ficcionEncontrada.GenerarDot());
else
    Console.WriteLine("Categoria no encontrada.");

Directory.CreateDirectory("wwwroot/graphs"); // crea la carpeta si no existe

Console.WriteLine("--- Generar imagen .png de Ficcion ---");
if (ficcionEncontrada != null)
{
    string ruta = ficcionEncontrada.GenerarImagen("wwwroot/graphs/ficcion.png");
    Console.WriteLine($"Imagen generada en: {ruta}");
}
//--Fin prueba--
*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();