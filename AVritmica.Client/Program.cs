using AVritmica.Client;
using AVritmica.Client.Servicios;
using AVritmica.Client.Servicios.Entidades;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7110")
});

// Servicio HTTP principal
builder.Services.AddScoped<IHttpServicio, HttpServicio>();

// Servicios de entidades 
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ICarritoServicio, CarritoServicio>();
builder.Services.AddScoped<ICarritoProductoServicio, CarritoProductoServicio>();
builder.Services.AddScoped<ICompraServicio, CompraServicio>();
builder.Services.AddScoped<ICompraDetalleServicio, CompraDetalleServicio>();
builder.Services.AddScoped<IConsultaServicio, ConsultaServicio>();
builder.Services.AddScoped<IPagoServicio, PagoServicio>();
builder.Services.AddScoped<IStockMovimientoServicio, StockMovimientoServicio>();
builder.Services.AddScoped<IVentaServicio, VentaServicio>();
builder.Services.AddScoped<ISesionUsuarioServicio, SesionUsuarioServicio>();


await builder.Build().RunAsync();
