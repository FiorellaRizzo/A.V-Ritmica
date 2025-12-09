
using AVritmica.BD.Data.Entity;

namespace AVritmica.Server.Repositorio
{
    public interface IVentaRepositorio
    {
        // Métodos básicos 
        Task<List<Venta>> Select();
        Task<Venta?> SelectById(int id);
        Task<int> Insert(Venta entidad);
        Task<bool> Update(int id, Venta entidad);
        Task<bool> Delete(int id);
        Task<bool> Existe(int id);

        // Métodos específicos para ventas
        Task<List<Venta>> SelectByFecha(DateTime fecha);
        Task<List<Venta>> SelectByRangoFechas(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Venta>> SelectByEstado(string estado);
        Task<List<Venta>> SelectByCliente(string nombreCliente);
        Task<List<Venta>> SelectByMetodoPago(string metodoPago);

        // Métodos para cálculos
        Task<decimal> ObtenerTotalVenta(int id);
        Task<int> ObtenerCantidadTotalProductos(int id);
        Task<decimal> ObtenerVentasTotalesPorPeriodo(DateTime fechaInicio, DateTime fechaFin);

        // Métodos específicos del negocio
        Task<bool> ActualizarEstado(int id, string estado);
        Task<bool> RegistrarPago(int id, DateTime fechaPago, string numeroComprobante);
        Task<bool> RegistrarEnvio(int id, string numeroTracking, string urlTracking);
    }
}