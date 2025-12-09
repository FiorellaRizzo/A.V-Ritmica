using Microsoft.EntityFrameworkCore;
using AVritmica.BD.Data;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;

namespace AVritmica.Server.RepositorioImplementacion
{
    public class CarritoRepositorio : ICarritoRepositorio
    {
        private readonly Context _context;

        public CarritoRepositorio(Context context)
        {
            _context = context;
        }

        public async Task<List<Carrito>> Select()
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .ToListAsync();
        }

        public async Task<Carrito?> SelectById(int id)
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Carrito>> SelectByUsuario(int usuarioId)
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Carrito>> SelectByEstado(string estado)
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .Where(x => x.Estado == estado)
                .ToListAsync();
        }

        public async Task<List<Carrito>> SelectByEstadoPago(string estadoPago)
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .Where(x => x.EstadoPago == estadoPago)
                .ToListAsync();
        }

        public async Task<Carrito?> SelectCarritoActivoByUsuario(int usuarioId)
        {
            return await _context.Carritos
                .Include(c => c.Usuario)
                .Include(c => c.CarritoProductos)
                    .ThenInclude(cp => cp.Producto)
                .Include(c => c.Pagos)
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId && x.Estado == "Activo");
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.Carritos
                .AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExisteCarritoActivo(int usuarioId)
        {
            return await _context.Carritos
                .AnyAsync(x => x.UsuarioId == usuarioId && x.Estado == "Activo");
        }

        public async Task<int> Insert(Carrito entidad)
        {
            await _context.Carritos.AddAsync(entidad);
            await _context.SaveChangesAsync();
            return entidad.Id;
        }

        public async Task<bool> Update(int id, Carrito entidad)
        {
            var carritoExistente = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carritoExistente == null)
                return false;

            carritoExistente.UsuarioId = entidad.UsuarioId;
            carritoExistente.Estado = entidad.Estado;
            carritoExistente.EstadoPago = entidad.EstadoPago;
            carritoExistente.MontoTotal = entidad.MontoTotal;
            carritoExistente.Saldo = entidad.Saldo;
            carritoExistente.DireccionEnvio = entidad.DireccionEnvio;
            carritoExistente.FechaConfirmacion = entidad.FechaConfirmacion;

            _context.Carritos.Update(carritoExistente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var carrito = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
                return false;

            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarEstado(int id, string estado)
        {
            var carrito = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
                return false;

            carrito.Estado = estado;

            if (estado == "Confirmado")
            {
                carrito.FechaConfirmacion = DateTime.UtcNow;
            }

            _context.Carritos.Update(carrito);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarEstadoPago(int id, string estadoPago)
        {
            var carrito = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
                return false;

            carrito.EstadoPago = estadoPago;
            _context.Carritos.Update(carrito);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarCarrito(int id, decimal montoTotal, string direccionEnvio)
        {
            var carrito = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
                return false;

            carrito.Estado = "Confirmado";
            carrito.MontoTotal = montoTotal;
            carrito.Saldo = montoTotal; // El saldo inicial es igual al monto total
            carrito.DireccionEnvio = direccionEnvio;
            carrito.FechaConfirmacion = DateTime.UtcNow;

            _context.Carritos.Update(carrito);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarMontoTotal(int id, decimal montoTotal)
        {
            var carrito = await _context.Carritos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carrito == null)
                return false;

            carrito.MontoTotal = montoTotal;
            // Actualizar el saldo también si es necesario
            carrito.Saldo = montoTotal - carrito.Pagos.Where(p => p.EstadoPago == "Aprobado").Sum(p => p.MontoPagado);

            _context.Carritos.Update(carrito);
            await _context.SaveChangesAsync();
            return true;
        }

        // En CarritoRepositorio.cs, al final de la clase
        public async Task<bool> AgregarProductoAlCarrito(int carritoId, int productoId, int cantidad,
            decimal precioUnitario, string? color = null, string? tamaño = null)
        {
            try
            {
                var carrito = await _context.Carritos
                    .Include(c => c.CarritoProductos)
                    .FirstOrDefaultAsync(c => c.Id == carritoId);

                if (carrito == null) return false;

                // Verificar si ya existe el producto con las mismas variantes
                var itemExistente = carrito.CarritoProductos.FirstOrDefault(cp =>
                    cp.ProductoId == productoId &&
                    cp.Color == color &&
                    cp.Tamaño == tamaño);

                if (itemExistente != null)
                {
                    // Actualizar cantidad si ya existe
                    itemExistente.Cantidad += cantidad;
                }
                else
                {
                    // Agregar nuevo item
                    carrito.CarritoProductos.Add(new CarritoProducto
                    {
                        ProductoId = productoId,
                        Cantidad = cantidad,
                        PrecioUnitario = precioUnitario,
                        Color = color,
                        Tamaño = tamaño
                    });
                }

                // Recalcular total
                carrito.MontoTotal = carrito.CarritoProductos.Sum(cp => cp.Cantidad * cp.PrecioUnitario);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarProductoAlCarrito: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarCantidadProducto(int carritoId, int productoId, int cantidad,
    string? color = null, string? tamaño = null)
        {
            try
            {
                Console.WriteLine($"🔄 ActualizarCantidadProducto: CarritoId={carritoId}, ProductoId={productoId}, Cantidad={cantidad}");

                // ✅ BUSCAR DIRECTAMENTE
                var item = await _context.CarritoProductos
                    .FirstOrDefaultAsync(cp =>
                        cp.CarritoId == carritoId &&
                        cp.ProductoId == productoId &&
                        cp.Color == color &&
                        cp.Tamaño == tamaño);

                if (item == null)
                {
                    Console.WriteLine($"❌ Producto no encontrado");
                    return false;
                }

                Console.WriteLine($"📝 Actualizando cantidad de {item.Cantidad} a {cantidad}");

                if (cantidad <= 0)
                {
                    // ✅ ELIMINAR directamente si cantidad es 0 o negativa
                    _context.CarritoProductos.Remove(item);
                    Console.WriteLine($"🗑️ Cantidad <= 0, eliminando producto");
                }
                else
                {
                    item.Cantidad = cantidad;
                    Console.WriteLine($"📊 Cantidad actualizada a {cantidad}");
                }

                // ✅ Recalcular total DEL CARRO COMPLETO
                var nuevoTotal = await _context.CarritoProductos
                    .Where(cp => cp.CarritoId == carritoId)
                    .SumAsync(cp => cp.Cantidad * cp.PrecioUnitario);

                // Actualizar carrito
                var carrito = await _context.Carritos.FindAsync(carritoId);
                if (carrito != null)
                {
                    carrito.MontoTotal = nuevoTotal;
                    Console.WriteLine($"💰 Nuevo total del carrito: {nuevoTotal}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Cantidad actualizada exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ActualizarCantidadProducto: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> EliminarProductoDelCarrito(int carritoId, int productoId,
        string? color = null, string? tamaño = null)
        {
            try
            {
                Console.WriteLine($"🔍 Buscando producto: CarritoId={carritoId}, ProductoId={productoId}, Color={color}, Tamaño={tamaño}");

                //  BUSCAR DIRECTAMENTE en la tabla CarritoProductos
                var item = await _context.CarritoProductos
                    .FirstOrDefaultAsync(cp =>
                        cp.CarritoId == carritoId &&
                        cp.ProductoId == productoId &&
                        cp.Color == color &&
                        cp.Tamaño == tamaño);

                if (item == null)
                {
                    Console.WriteLine($"❌ Producto no encontrado en la tabla CarritoProductos");
                    return false;
                }

                Console.WriteLine($"✅ Encontrado item Id={item.Id}, eliminando...");

                //  ELIMINAR DIRECTAMENTE (no usando la navegación carrito.CarritoProductos)
                _context.CarritoProductos.Remove(item);

                //  Recalcular total DEL CARRO COMPLETO
                var nuevoTotal = await _context.CarritoProductos
                    .Where(cp => cp.CarritoId == carritoId)
                    .SumAsync(cp => cp.Cantidad * cp.PrecioUnitario);

                // Actualizar carrito
                var carrito = await _context.Carritos.FindAsync(carritoId);
                if (carrito != null)
                {
                    carrito.MontoTotal = nuevoTotal;
                    Console.WriteLine($"💰 Nuevo total del carrito: {nuevoTotal}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Producto eliminado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en EliminarProductoDelCarrito: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> VaciarCarrito(int carritoId)
        {
            try
            {
                Console.WriteLine($"🧹 VaciarCarrito para carritoId: {carritoId}");

                // ELIMINAR TODOS los items de ESTE carrito directamente
                var items = await _context.CarritoProductos
                    .Where(cp => cp.CarritoId == carritoId)
                    .ToListAsync();

                Console.WriteLine($"📊 Encontrados {items.Count} items para eliminar");

                if (items.Any())
                {
                    _context.CarritoProductos.RemoveRange(items);
                    Console.WriteLine($"🗑️ Eliminando {items.Count} items...");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Carrito ya está vacío");
                }

                // Actualizar carrito (poner total en 0)
                var carrito = await _context.Carritos.FindAsync(carritoId);
                if (carrito != null)
                {
                    carrito.MontoTotal = 0;
                    Console.WriteLine($"💰 Total del carrito puesto a 0");
                }
                else
                {
                    Console.WriteLine($"⚠️ Carrito {carritoId} no encontrado");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Carrito vaciado exitosamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en VaciarCarrito: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
