using SistemaVenta.IOC;
using SistemaVenta.AplicacionWeb.Utilidades.Automapper;
using SistemaVenta.AplicacionWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.InyectarDependencia(builder.Configuration);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Carga la libreria nativa de DinkToPdf segun el sistema operativo
// En Docker (Linux) carga el .so, en Windows el .dll, en Mac el .dylib
var context = new CustomAssemblyLoadContext();
string libreariaPDF;
if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
    libreariaPDF = "Utilidades/LibreriaPDF/libwkhtmltox.so";
else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
    libreariaPDF = "Utilidades/LibreriaPDF/libwkhtmltox.dylib";
else
    libreariaPDF = "Utilidades/LibreriaPDF/libwkhtmltox.dll";
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), libreariaPDF));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
