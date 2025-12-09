
using AVritmica.BD.Data.Entity;
using AVritmica.Shared.DTO;

namespace AVritmica.Client.Servicios.Entidades
{
    public class VentaServicio : IVentaServicio
    {
        private readonly IHttpServicio http;
        private string url = "api/Ventas";

        public VentaServicio(IHttpServicio http) => this.http = http;

        // Métodos básicos 
        public async Task<HttpRespuesta<List<Venta>>> Get() => await http.Get<List<Venta>>(url);
        public async Task<HttpRespuesta<Venta>> Get(int id) => await http.Get<Venta>($"{url}/{id}");
        public async Task<HttpRespuesta<List<Venta>>> GetByFecha(DateTime fecha) => await http.Get<List<Venta>>($"{url}/GetByFecha/{fecha:yyyy-MM-dd}");
        public async Task<HttpRespuesta<List<Venta>>> GetByRangoFechas(DateTime fechaInicio, DateTime fechaFin) => await http.Get<List<Venta>>($"{url}/GetByRangoFechas?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}");
        public async Task<HttpRespuesta<object>> Post(Venta entidad) => await http.Post(url, entidad);
        public async Task<HttpRespuesta<object>> Put(Venta entidad) => await http.Put($"{url}/{entidad.Id}", entidad);
        public async Task<HttpRespuesta<object>> Delete(int id) => await http.Delete($"{url}/{id}");

        // Métodos específicos de ventas
        public async Task<HttpRespuesta<decimal>> ObtenerTotalVenta(int id) => await http.Get<decimal>($"{url}/total/{id}");
        public async Task<HttpRespuesta<int>> ObtenerCantidadTotalProductos(int id) => await http.Get<int>($"{url}/cantidad-productos/{id}");
        public async Task<HttpRespuesta<List<Venta>>> GetByEstado(string estado) => await http.Get<List<Venta>>($"{url}/por-estado/{estado}");
        public async Task<HttpRespuesta<List<Venta>>> GetByCliente(string nombre) => await http.Get<List<Venta>>($"{url}/por-cliente/{nombre}");
        public async Task<HttpRespuesta<List<Venta>>> GetByMetodoPago(string metodo) => await http.Get<List<Venta>>($"{url}/por-metodo-pago/{metodo}");

        // Métodos nuevos
        public async Task<HttpRespuesta<object>> ActualizarEstado(int id, string estado) => await http.Put($"{url}/{id}/estado", estado);
        public async Task<HttpRespuesta<object>> RegistrarPago(int id, DateTime fechaPago, string numeroComprobante)
        {
            var request = new { FechaPago = fechaPago, NumeroComprobante = numeroComprobante };
            return await http.Post($"{url}/{id}/registrar-pago", request);
        }

        public async Task<HttpRespuesta<object>> RegistrarEnvio(int id, string numeroTracking, string urlTracking)
        {
            var request = new { NumeroTracking = numeroTracking, UrlTracking = urlTracking };
            return await http.Post($"{url}/{id}/registrar-envio", request);
        }

        // CHECKOUT - Método especial usando DTO
        public async Task<HttpRespuesta<object>> CrearVentaConDetalles(CrearVentaDTO ventaDto)
        {
            return await http.Post($"{url}/crear-con-detalles", ventaDto);
        }
    }
}