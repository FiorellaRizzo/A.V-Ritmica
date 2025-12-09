using Microsoft.EntityFrameworkCore;
using AVritmica.BD.Data;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;

namespace AVritmica.Server.RepositorioImplementacion
{
    public class CarritoProductoRepositorio : ICarritoProductoRepositorio
    {
        private readonly Context _context;

        public CarritoProductoRepositorio(Context context)
        {
            _context = context;
        }

        // ========== MÉTODOS EXISTENTES (sin cambios) ==========

        public async Task<List<CarritoProducto>> Select()
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<CarritoProducto?> SelectById(int id)
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CarritoProducto>> SelectByCarrito(int carritoId)
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(x => x.CarritoId == carritoId)
                .ToListAsync();
        }

        public async Task<List<CarritoProducto>> SelectByProducto(int productoId)
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .Where(x => x.ProductoId == productoId)
                .ToListAsync();
        }

        // ========== MÉTODOS ACTUALIZADOS PARA VARIANTES ==========

        // Método ORIGINAL (mantener para productos sin variantes)
        public async Task<CarritoProducto?> SelectByCarritoAndProducto(int carritoId, int productoId)
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(x =>
                    x.CarritoId == carritoId &&
                    x.ProductoId == productoId &&
                    x.Color == null &&  // Solo productos sin variantes
                    x.Tamaño == null);
        }

        // NUEVO MÉTODO con soporte para variantes
        public async Task<CarritoProducto?> SelectByCarritoAndProductoConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null)
        {
            return await _context.CarritoProductos
                .Include(cp => cp.Carrito)
                .Include(cp => cp.Producto)
                    .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(x =>
                    x.CarritoId == carritoId &&
                    x.ProductoId == productoId &&
                    (x.Color == color || (x.Color == null && color == null)) &&
                    (x.Tamaño == tamaño || (x.Tamaño == null && tamaño == null)));
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.CarritoProductos
                .AnyAsync(x => x.Id == id);
        }

        // Método ORIGINAL (mantener para compatibilidad)
        public async Task<bool> Existe(int carritoId, int productoId)
        {
            return await _context.CarritoProductos
                .AnyAsync(x =>
                    x.CarritoId == carritoId &&
                    x.ProductoId == productoId &&
                    x.Color == null &&
                    x.Tamaño == null);
        }

        // NUEVO MÉTODO con soporte para variantes
        public async Task<bool> ExisteConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null)
        {
            return await _context.CarritoProductos
                .AnyAsync(x =>
                    x.CarritoId == carritoId &&
                    x.ProductoId == productoId &&
                    (x.Color == color || (x.Color == null && color == null)) &&
                    (x.Tamaño == tamaño || (x.Tamaño == null && tamaño == null)));
        }

        public async Task<int> Insert(CarritoProducto entidad)
        {
            try
            {
                // Verificar si ya existe el producto con las mismas variantes
                var existente = await SelectByCarritoAndProductoConVariantes(
                    entidad.CarritoId,
                    entidad.ProductoId,
                    entidad.Color,
                    entidad.Tamaño);

                if (existente != null)
                {
                    // Si ya existe, actualizar la cantidad
                    existente.Cantidad += entidad.Cantidad;
                    _context.CarritoProductos.Update(existente);
                    await _context.SaveChangesAsync();
                    return existente.Id;
                }

                await _context.CarritoProductos.AddAsync(entidad);
                await _context.SaveChangesAsync();
                return entidad.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Insert: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> Update(int id, CarritoProducto entidad)
        {
            try
            {
                var carritoProductoExistente = await _context.CarritoProductos
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (carritoProductoExistente == null)
                    return false;

                // Verificar si se está cambiando a una combinación que ya existe
                if (carritoProductoExistente.CarritoId != entidad.CarritoId ||
                    carritoProductoExistente.ProductoId != entidad.ProductoId ||
                    carritoProductoExistente.Color != entidad.Color ||
                    carritoProductoExistente.Tamaño != entidad.Tamaño)
                {
                    var existe = await ExisteConVariantes(
                        entidad.CarritoId,
                        entidad.ProductoId,
                        entidad.Color,
                        entidad.Tamaño);

                    if (existe && carritoProductoExistente.Id != id)
                    {
                        return false; // Ya existe esa combinación
                    }
                }

                // Actualizar propiedades
                carritoProductoExistente.CarritoId = entidad.CarritoId;
                carritoProductoExistente.ProductoId = entidad.ProductoId;
                carritoProductoExistente.Cantidad = entidad.Cantidad;
                carritoProductoExistente.PrecioUnitario = entidad.PrecioUnitario;
                carritoProductoExistente.Color = entidad.Color;
                carritoProductoExistente.Tamaño = entidad.Tamaño;

                _context.CarritoProductos.Update(carritoProductoExistente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Update: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var carritoProducto = await _context.CarritoProductos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carritoProducto == null)
                return false;

            _context.CarritoProductos.Remove(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        // Metodo original (mantener para compatibilidad)
        public async Task<bool> DeleteByCarritoAndProducto(int carritoId, int productoId)
        {
            var carritoProducto = await _context.CarritoProductos
                .FirstOrDefaultAsync(x =>
                    x.CarritoId == carritoId &&
                    x.ProductoId == productoId &&
                    x.Color == null &&
                    x.Tamaño == null);

            if (carritoProducto == null)
                return false;

            _context.CarritoProductos.Remove(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        //  Metodo con soporte para variantes
        public async Task<bool> DeleteByCarritoAndProductoConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null)
        {
            var carritoProducto = await SelectByCarritoAndProductoConVariantes(
                carritoId, productoId, color, tamaño);

            if (carritoProducto == null)
                return false;

            _context.CarritoProductos.Remove(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarCantidad(int id, int cantidad)
        {
            var carritoProducto = await _context.CarritoProductos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carritoProducto == null)
                return false;

            carritoProducto.Cantidad = cantidad;

            // No permitir cantidad negativa
            if (carritoProducto.Cantidad < 0)
                carritoProducto.Cantidad = 0;

            _context.CarritoProductos.Update(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        //  Actualizar cantidad considerando variantes
        public async Task<bool> ActualizarCantidadConVariantes(
            int carritoId,
            int productoId,
            int cantidad,
            string? color = null,
            string? tamaño = null)
        {
            var carritoProducto = await SelectByCarritoAndProductoConVariantes(
                carritoId, productoId, color, tamaño);

            if (carritoProducto == null)
                return false;

            if (cantidad <= 0)
            {
                // Eliminar si la cantidad es 0 o negativa
                return await DeleteByCarritoAndProductoConVariantes(
                    carritoId, productoId, color, tamaño);
            }

            carritoProducto.Cantidad = cantidad;
            _context.CarritoProductos.Update(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarPrecioUnitario(int id, decimal precioUnitario)
        {
            var carritoProducto = await _context.CarritoProductos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carritoProducto == null)
                return false;

            carritoProducto.PrecioUnitario = precioUnitario;

            // No permitir precio negativo
            if (carritoProducto.PrecioUnitario < 0)
                carritoProducto.PrecioUnitario = 0;

            _context.CarritoProductos.Update(carritoProducto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> ObtenerCantidadTotalEnCarritos(int productoId)
        {
            return await _context.CarritoProductos
                .Where(x => x.ProductoId == productoId)
                .SumAsync(x => x.Cantidad);
        }

        //  MÉTODOS PARA EL CARRITO 

        public async Task<bool> VaciarCarrito(int carritoId)
        {
            try
            {
                var items = await SelectByCarrito(carritoId);

                foreach (var item in items)
                {
                    _context.CarritoProductos.Remove(item);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en VaciarCarrito: {ex.Message}");
                return false;
            }
        }

        public async Task<int> ObtenerCantidadTotalPorCarrito(int carritoId)
        {
            return await _context.CarritoProductos
                .Where(x => x.CarritoId == carritoId)
                .SumAsync(x => x.Cantidad);
        }

        public async Task<decimal> ObtenerTotalPorCarrito(int carritoId)
        {
            return await _context.CarritoProductos
                .Where(x => x.CarritoId == carritoId)
                .SumAsync(x => x.Cantidad * x.PrecioUnitario);
        }

        public async Task<bool> AgregarOActualizarProducto(
            int carritoId,
            int productoId,
            int cantidad,
            decimal precioUnitario,
            string? color = null,
            string? tamaño = null)
        {
            try
            {
                var existente = await SelectByCarritoAndProductoConVariantes(
                    carritoId, productoId, color, tamaño);

                if (existente != null)
                {
                    // Actualizar cantidad del existente
                    existente.Cantidad += cantidad;
                    return await Update(existente.Id, existente);
                }
                else
                {
                    // Crear nuevo item
                    var nuevoItem = new CarritoProducto
                    {
                        CarritoId = carritoId,
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        Color = color,
                        Tamaño = tamaño
                    };

                    await Insert(nuevoItem);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarOActualizarProducto: {ex.Message}");
                return false;
            }
        }
    }
}