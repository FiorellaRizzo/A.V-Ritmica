using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using AVritmica.BD.Data;
using AVritmica.Server.Repositorio;
using AVritmica.Server.RepositorioImplementacion;

//------------------------------------------------------------------
//configuracion de los servicios en el constructor de la aplicaci�n
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(
    x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(op => op.UseSqlServer("name=conn"));//

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ICarritoRepositorio, CarritoRepositorio>();
builder.Services.AddScoped<ICarritoProductoRepositorio, CarritoProductoRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<ICompraRepositorio, CompraRepositorio>();
builder.Services.AddScoped<ICompraDetalleRepositorio, CompraDetalleRepositorio>();
builder.Services.AddScoped<IConsultaRepositorio, ConsultaRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<IReportesRepositorio, ReportesRepositorio>();
builder.Services.AddScoped<IStockMovimientoRepositorio, StockMovimientoRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IVentaRepositorio, VentaRepositorio>();

//--------------------------------------------------------------------
//construcc�n de la aplicaci�n
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
