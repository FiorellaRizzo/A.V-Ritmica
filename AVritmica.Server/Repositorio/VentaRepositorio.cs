
using AVritmica.BD.Data;
using AVritmica.BD.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace AVritmica.Server.Repositorio
{
    public class VentaRepositorio : IVentaRepositorio
    {
        private readonly Context _context;

        public VentaRepositorio(Context context)
        {
            _context = context;
        }

        // SELECT - Todas las ventas
        public async Task<List<Venta>> Select()
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // SELECT BY ID
        public async Task<Venta?> SelectById(int id)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // INSERT
        public async Task<int> Insert(Venta entidad)
        {
            // Generar número de venta si no tiene
            if (string.IsNullOrEmpty(entidad.NumeroVenta))
            {
                var ultimoNumero = await _context.Ventas
                    .OrderByDescending(v => v.Id)
                    .Select(v => v.Id)
                    .FirstOrDefaultAsync();

                entidad.GenerarNumeroVenta(ultimoNumero);
            }

            // Establecer fecha si no tiene
            if (entidad.Fecha == default)
            {
                entidad.Fecha = DateTime.Now;
            }

            _context.Ventas.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad.Id;
        }

        // UPDATE
        public async Task<bool> Update(int id, Venta entidad)
        {
            var ventaExistente = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (ventaExistente == null)
                return false;

            // Actualizar propiedades básicas
            ventaExistente.Estado = entidad.Estado;
            ventaExistente.Notas = entidad.Notas;
            ventaExistente.NumeroTracking = entidad.NumeroTracking;
            ventaExistente.UrlTracking = entidad.UrlTracking;
            ventaExistente.FechaPago = entidad.FechaPago;
            ventaExistente.NumeroComprobante = entidad.NumeroComprobante;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return false;

            // Solo permitir borrar ventas canceladas o muy recientes
            if (venta.Estado != "Cancelada" && venta.Fecha < DateTime.Now.AddDays(-1))
            {
                return false; // No se puede borrar ventas activas o antiguas
            }

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
            return true;
        }

        // EXISTE
        public async Task<bool> Existe(int id)
        {
            return await _context.Ventas.AnyAsync(v => v.Id == id);
        }

        // SELECT BY FECHA
        public async Task<List<Venta>> SelectByFecha(DateTime fecha)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.Fecha.Date == fecha.Date)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // SELECT BY RANGO FECHAS
        public async Task<List<Venta>> SelectByRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.Fecha.Date >= fechaInicio.Date && v.Fecha.Date <= fechaFin.Date)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // SELECT BY ESTADO
        public async Task<List<Venta>> SelectByEstado(string estado)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.Estado == estado)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // SELECT BY CLIENTE
        public async Task<List<Venta>> SelectByCliente(string nombreCliente)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.NombreCliente.Contains(nombreCliente))
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // SELECT BY METODO PAGO
        public async Task<List<Venta>> SelectByMetodoPago(string metodoPago)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.MetodoPago == metodoPago)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        // OBTENER TOTAL VENTA
        public async Task<decimal> ObtenerTotalVenta(int id)
        {
            var venta = await SelectById(id);
            return venta?.Total ?? 0;
        }

        // OBTENER CANTIDAD TOTAL PRODUCTOS
        public async Task<int> ObtenerCantidadTotalProductos(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            return venta?.Detalles.Sum(d => d.Cantidad) ?? 0;
        }

        // OBTENER VENTAS TOTALES POR PERIODO
        public async Task<decimal> ObtenerVentasTotalesPorPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Ventas
                .Where(v => v.Fecha.Date >= fechaInicio.Date && v.Fecha.Date <= fechaFin.Date && v.Estado != "Cancelada")
                .SumAsync(v => v.Total);
        }

        // ACTUALIZAR ESTADO
        public async Task<bool> ActualizarEstado(int id, string estado)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return false;

            venta.Estado = estado;
            await _context.SaveChangesAsync();
            return true;
        }

        // REGISTRAR PAGO
        public async Task<bool> RegistrarPago(int id, DateTime fechaPago, string numeroComprobante)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return false;

            venta.FechaPago = fechaPago;
            venta.NumeroComprobante = numeroComprobante;
            venta.Estado = "Pagada";

            await _context.SaveChangesAsync();
            return true;
        }

        // REGISTRAR ENVÍO
        public async Task<bool> RegistrarEnvio(int id, string numeroTracking, string urlTracking)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return false;

            venta.NumeroTracking = numeroTracking;
            venta.UrlTracking = urlTracking;
            venta.Estado = "Enviada";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}