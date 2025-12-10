using AVritmica.BD.Data.Entity;
using AVritmica.Shared.DTO;
using AVritmica.Client.Servicios;

namespace AVritmica.Client.Servicios.Entidades
{
    public class CarritoServicio : ICarritoServicio
    {
        private readonly IHttpServicio http;
        private readonly ISesionUsuarioServicio sesionUsuarioServicio;

        private const string url = "api/Carritos";
        private const int UsuarioPorDefecto = 1;

        public CarritoServicio(IHttpServicio http, ISesionUsuarioServicio sesionUsuarioServicio)
        {
            this.http = http;
            this.sesionUsuarioServicio = sesionUsuarioServicio;
        }

        
        /// Devuelve el usuario actual elegido en el selector o 1 si no hay nada guardado en localStorage.
        
        private async Task<int> ObtenerUsuarioIdAsync()
        {
            var id = await sesionUsuarioServicio.ObtenerUsuarioIdAsync();
            return id ?? UsuarioPorDefecto;
        }

        // MÉTODOS EXISTENTES 

        public async Task<HttpRespuesta<List<Carrito>>> Get()
            => await http.Get<List<Carrito>>(url);

        public async Task<HttpRespuesta<Carrito>> Get(int id)
            => await http.Get<Carrito>($"{url}/{id}");

        public async Task<HttpRespuesta<List<Carrito>>> GetByUsuario(int usuarioId)
            => await http.Get<List<Carrito>>($"{url}/GetByUsuario/{usuarioId}");

        public async Task<HttpRespuesta<List<Carrito>>> GetByEstado(string estado)
            => await http.Get<List<Carrito>>($"{url}/GetByEstado/{estado}");

        public async Task<HttpRespuesta<Carrito>> GetCarritoActivo(int usuarioId)
            => await http.Get<Carrito>($"{url}/GetCarritoActivo/{usuarioId}");

        public async Task<HttpRespuesta<object>> Post(Carrito entidad)
            => await http.Post(url, entidad);

        public async Task<HttpRespuesta<object>> Put(Carrito entidad)
            => await http.Put($"{url}/{entidad.Id}", entidad);

        public async Task<HttpRespuesta<object>> Delete(int id)
            => await http.Delete($"{url}/{id}");

        public async Task<HttpRespuesta<object>> ActualizarEstado(int id, string estado)
            => await http.Post($"{url}/actualizar-estado/{id}", estado);

        public async Task<HttpRespuesta<object>> ConfirmarCarrito(int id, decimal montoTotal, string direccionEnvio)
            => await http.Post($"{url}/confirmar-carrito/{id}", new { montoTotal, direccionEnvio });

        //  MÉTODOS NUEVOS (con usuario actual) 

        public async Task<HttpRespuesta<object>> AgregarItem(AgregarAlCarritoDTO item)
        {
            //  DTO con el usuario actual
            item.UsuarioId = await ObtenerUsuarioIdAsync();

            
            return await http.Post($"{url}/AgregarItem", item);
        }

        public async Task<HttpRespuesta<List<CarritoItemDTO>>> ObtenerItemsCarrito()
        {
            var usuarioId = await ObtenerUsuarioIdAsync();
            return await http.Get<List<CarritoItemDTO>>($"{url}/items?usuarioId={usuarioId}");
        }

        public async Task<HttpRespuesta<object>> ActualizarCantidadItem(int productoId, int cantidad, string color, string tamaño)
        {
            var usuarioId = await ObtenerUsuarioIdAsync();

            var dto = new
            {
                UsuarioId = usuarioId,
                ProductoId = productoId,
                Cantidad = cantidad,
                Color = color,
                Tamaño = tamaño
            };

            return await http.Put($"{url}/actualizar-cantidad", dto);
        }

        public async Task<HttpRespuesta<object>> EliminarItem(int productoId, string color, string tamaño)
        {
            var usuarioId = await ObtenerUsuarioIdAsync();

            return await http.Delete(
                $"{url}/eliminar-item/{productoId}?usuarioId={usuarioId}&color={color}&tamaño={tamaño}");
        }

        public async Task<HttpRespuesta<object>> VaciarCarrito()
        {
            var usuarioId = await ObtenerUsuarioIdAsync();
            return await http.Delete($"{url}/vaciar?usuarioId={usuarioId}");
        }
    }
}
