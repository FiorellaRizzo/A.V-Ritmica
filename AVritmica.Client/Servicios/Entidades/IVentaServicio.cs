
using AVritmica.BD.Data.Entity;
using AVritmica.Client.Servicios;
using AVritmica.Shared.DTO;

namespace AVritmica.Client.Servicios.Entidades
{
    public interface IVentaServicio
    {
        // Métodos básicos CRUD (EXACTAMENTE como ICompraServicio)
        Task<HttpRespuesta<List<Venta>>> Get();
        Task<HttpRespuesta<Venta>> Get(int id);
        Task<HttpRespuesta<List<Venta>>> GetByFecha(DateTime fecha);
        Task<HttpRespuesta<List<Venta>>> GetByRangoFechas(DateTime fechaInicio, DateTime fechaFin);
        Task<HttpRespuesta<object>> Post(Venta entidad);
        Task<HttpRespuesta<object>> Put(Venta entidad);
        Task<HttpRespuesta<object>> Delete(int id);

        // Métodos específicos (SIMILARES a ICompraServicio)
        Task<HttpRespuesta<decimal>> ObtenerTotalVenta(int id);
        Task<HttpRespuesta<int>> ObtenerCantidadTotalProductos(int id);

        // Métodos adicionales para ventas
        Task<HttpRespuesta<List<Venta>>> GetByEstado(string estado);
        Task<HttpRespuesta<List<Venta>>> GetByCliente(string nombre);
        Task<HttpRespuesta<List<Venta>>> GetByMetodoPago(string metodo);

        // Métodos para operaciones específicas
        Task<HttpRespuesta<object>> ActualizarEstado(int id, string estado);
        Task<HttpRespuesta<object>> RegistrarPago(int id, DateTime fechaPago, string numeroComprobante);
        Task<HttpRespuesta<object>> RegistrarEnvio(int id, string numeroTracking, string urlTracking);

        // CHECKOUT - Método especial
        Task<HttpRespuesta<object>> CrearVentaConDetalles(CrearVentaDTO ventaDto);
    }
}