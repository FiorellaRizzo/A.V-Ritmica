using AVritmica.BD.Data.Entity;
using AVritmica.Shared.DTO;
using AVritmica.Client.Servicios;

namespace AVritmica.Client.Servicios.Entidades
{
    public class CarritoServicio : ICarritoServicio
    {
        private readonly IHttpServicio http;
        private string url = "api/Carritos";
        private int usuarioIdTemporal = 1; // Temporal hasta que haga lo del login

        public CarritoServicio(IHttpServicio http) => this.http = http;

        //MÉTODOS EXISTENTES 
        public async Task<HttpRespuesta<List<Carrito>>> Get() => await http.Get<List<Carrito>>(url);
        public async Task<HttpRespuesta<Carrito>> Get(int id) => await http.Get<Carrito>($"{url}/{id}");
        public async Task<HttpRespuesta<List<Carrito>>> GetByUsuario(int usuarioId) => await http.Get<List<Carrito>>($"{url}/GetByUsuario/{usuarioId}");
        public async Task<HttpRespuesta<List<Carrito>>> GetByEstado(string estado) => await http.Get<List<Carrito>>($"{url}/GetByEstado/{estado}");
        public async Task<HttpRespuesta<Carrito>> GetCarritoActivo(int usuarioId) => await http.Get<Carrito>($"{url}/GetCarritoActivo/{usuarioId}");
        public async Task<HttpRespuesta<object>> Post(Carrito entidad) => await http.Post(url, entidad);
        public async Task<HttpRespuesta<object>> Put(Carrito entidad) => await http.Put($"{url}/{entidad.Id}", entidad);
        public async Task<HttpRespuesta<object>> Delete(int id) => await http.Delete($"{url}/{id}");
        public async Task<HttpRespuesta<object>> ActualizarEstado(int id, string estado) => await http.Post($"{url}/actualizar-estado/{id}", estado);
        public async Task<HttpRespuesta<object>> ConfirmarCarrito(int id, decimal montoTotal, string direccionEnvio) => await http.Post($"{url}/confirmar-carrito/{id}", new { montoTotal, direccionEnvio });

        //  MÉTODOS NUEVOS - ACTUALIZADOS CON LO DEL USUARIO TEMPORTAL

        
        public async Task<HttpRespuesta<object>> AgregarItem(AgregarAlCarritoDTO item)
        {
            return await http.Post($"{url}/AgregarItem", item);
        }

       
        public async Task<HttpRespuesta<List<CarritoItemDTO>>> ObtenerItemsCarrito()
        {
            return await http.Get<List<CarritoItemDTO>>($"{url}/items?usuarioId={usuarioIdTemporal}");
        }

      
        public async Task<HttpRespuesta<object>> ActualizarCantidadItem(int productoId, int cantidad, string color, string tamaño)
        {
            var dto = new
            {
                UsuarioId = usuarioIdTemporal,  // ESTO ES LO DEL USUARIO QUE ES TEMPORAL DESPUES CUANDO HAGA EL LOGIN LO ELIMINO
                ProductoId = productoId,
                Cantidad = cantidad,
                Color = color,
                Tamaño = tamaño
            };
            return await http.Put($"{url}/actualizar-cantidad", dto);
        }

        
        public async Task<HttpRespuesta<object>> EliminarItem(int productoId, string color, string tamaño)
        {
            
            return await http.Delete($"{url}/eliminar-item/{productoId}?usuarioId={usuarioIdTemporal}&color={color}&tamaño={tamaño}");
        }

   
        public async Task<HttpRespuesta<object>> VaciarCarrito()
        {
            return await http.Delete($"{url}/vaciar?usuarioId={usuarioIdTemporal}");
        }
    }
}