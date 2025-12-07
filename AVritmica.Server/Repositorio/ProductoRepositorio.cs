using Microsoft.EntityFrameworkCore;
using AVritmica.BD.Data;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;

namespace AVritmica.Server.RepositorioImplementacion
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly Context _context;

        public ProductoRepositorio(Context context)
        {
            _context = context;
        }

        public async Task<List<Producto>> Select()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<Producto?> SelectById(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Producto>> SelectByCategoria(int categoriaId)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(x => x.CategoriaId == categoriaId)
                .ToListAsync();
        }

        public async Task<Producto?> SelectByNombre(string nombre)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(x => x.Nombre == nombre);
        }

        public async Task<List<Producto>> SelectByPrecioRange(decimal precioMin, decimal precioMax)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(x => x.Precio >= precioMin && x.Precio <= precioMax)
                .ToListAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.Productos
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Existe(string nombre)
        {
            return await _context.Productos
                .AnyAsync(x => x.Nombre == nombre);
        }

        public async Task<int> Insert(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto.Id; // Devuelve el ID generado
        }

        public async Task<bool> Update(int id, Producto entidad)
        {
            var productoExistente = await _context.Productos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (productoExistente == null)
                return false;

            // ACTUALIZAR TODOS LOS CAMPOS (incluyendo los nuevos)
            productoExistente.Nombre = entidad.Nombre;
            productoExistente.Descripcion = entidad.Descripcion;
            productoExistente.Precio = entidad.Precio;
            productoExistente.Stock = entidad.Stock;
            productoExistente.ImagenUrl = entidad.ImagenUrl;
            productoExistente.CategoriaId = entidad.CategoriaId;

            // NUEVOS CAMPOS PARA VARIANTES (¡ESTOS FALTAN!)
            productoExistente.TieneVariantes = entidad.TieneVariantes;
            productoExistente.ColoresDisponibles = entidad.ColoresDisponibles;
            productoExistente.TamaniosDisponibles = entidad.TamaniosDisponibles;
            productoExistente.ImagenesVariantes = entidad.ImagenesVariantes;

            _context.Productos.Update(productoExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarStock(int id, int cantidad)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (producto == null)
                return false;

            producto.Stock += cantidad;

            // No permitir stock negativo
            if (producto.Stock < 0)
                producto.Stock = 0;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
