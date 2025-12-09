using AVritmica.BD.Data.Entity;
using AVritmica.Client.Servicios;
using AVritmica.Shared.DTO; // ¡Agregar este using!

namespace AVritmica.Client.Servicios.Entidades
{
    public interface ICarritoServicio
    {
        // Métodos existentes
        Task<HttpRespuesta<List<Carrito>>> Get();
        Task<HttpRespuesta<Carrito>> Get(int id);
        Task<HttpRespuesta<List<Carrito>>> GetByUsuario(int usuarioId);
        Task<HttpRespuesta<List<Carrito>>> GetByEstado(string estado);
        Task<HttpRespuesta<Carrito>> GetCarritoActivo(int usuarioId);
        Task<HttpRespuesta<object>> Post(Carrito entidad);
        Task<HttpRespuesta<object>> Put(Carrito entidad);
        Task<HttpRespuesta<object>> Delete(int id);
        Task<HttpRespuesta<object>> ActualizarEstado(int id, string estado);
        Task<HttpRespuesta<object>> ConfirmarCarrito(int id, decimal montoTotal, string direccionEnvio);

        // Nuevos métodos (agregar estas líneas)
        Task<HttpRespuesta<object>> AgregarItem(AgregarAlCarritoDTO item);
        Task<HttpRespuesta<List<CarritoItemDTO>>> ObtenerItemsCarrito();
        Task<HttpRespuesta<object>> ActualizarCantidadItem(int productoId, int cantidad, string color, string tamaño);
        Task<HttpRespuesta<object>> EliminarItem(int productoId, string color, string tamaño);
        Task<HttpRespuesta<object>> VaciarCarrito();
    }
}