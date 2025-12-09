using Microsoft.AspNetCore.Mvc;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;
using AVritmica.Shared.DTO;

namespace AVritmica.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaRepositorio repositorio;

        public VentasController(IVentaRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: api/Ventas
        [HttpGet]
        public async Task<ActionResult<List<Venta>>> Get()
        {
            return await repositorio.Select();
        }

        // GET: api/Ventas/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Venta>> Get(int id)
        {
            Venta? venta = await repositorio.SelectById(id);
            if (venta == null)
            {
                return NotFound();
            }
            return venta;
        }

        // GET: api/Ventas/GetByFecha/{fecha}
        [HttpGet("GetByFecha/{fecha:datetime}")]
        public async Task<ActionResult<List<Venta>>> GetByFecha(DateTime fecha)
        {
            var ventas = await repositorio.SelectByFecha(fecha);
            return ventas;
        }

        // GET: api/Ventas/GetByRangoFechas
        [HttpGet("GetByRangoFechas")]
        public async Task<ActionResult<List<Venta>>> GetByRangoFechas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
            {
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha fin");
            }

            var ventas = await repositorio.SelectByRangoFechas(fechaInicio, fechaFin);
            return ventas;
        }

        // GET: api/Ventas/existe/{id}
        [HttpGet("existe/{id:int}")]
        public async Task<ActionResult<bool>> Existe(int id)
        {
            return await repositorio.Existe(id);
        }

        // GET: api/Ventas/total/{id}
        [HttpGet("total/{id:int}")]
        public async Task<ActionResult<decimal>> ObtenerTotalVenta(int id)
        {
            var total = await repositorio.ObtenerTotalVenta(id);
            return total;
        }

        // GET: api/Ventas/cantidad-productos/{id}
        [HttpGet("cantidad-productos/{id:int}")]
        public async Task<ActionResult<int>> ObtenerCantidadTotalProductos(int id)
        {
            var cantidad = await repositorio.ObtenerCantidadTotalProductos(id);
            return cantidad;
        }

        // GET: api/Ventas/por-estado/{estado}
        [HttpGet("por-estado/{estado}")]
        public async Task<ActionResult<List<Venta>>> GetByEstado(string estado)
        {
            var ventas = await repositorio.SelectByEstado(estado);
            return ventas;
        }

        // GET: api/Ventas/por-cliente/{nombre}
        [HttpGet("por-cliente/{nombre}")]
        public async Task<ActionResult<List<Venta>>> GetByCliente(string nombre)
        {
            var ventas = await repositorio.SelectByCliente(nombre);
            return ventas;
        }

        // GET: api/Ventas/por-metodo-pago/{metodo}
        [HttpGet("por-metodo-pago/{metodo}")]
        public async Task<ActionResult<List<Venta>>> GetByMetodoPago(string metodo)
        {
            var ventas = await repositorio.SelectByMetodoPago(metodo);
            return ventas;
        }

        // POST: api/Ventas
        [HttpPost]
        public async Task<ActionResult<int>> Post(Venta entidad)
        {
            try
            {
                // Asegurar que la fecha sea la actual si no se especifica
                if (entidad.Fecha == default)
                {
                    entidad.Fecha = DateTime.Now;
                }

                // Si no tiene estado, asignar "Pendiente"
                if (string.IsNullOrEmpty(entidad.Estado))
                {
                    entidad.Estado = "Pendiente";
                }

                return await repositorio.Insert(entidad);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        // PUT: api/Ventas/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] Venta entidad)
        {
            try
            {
                if (id != entidad.Id)
                {
                    return BadRequest("Datos Incorrectos");
                }

                var resultado = await repositorio.Update(id, entidad);

                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar la venta");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Ventas/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resp = await repositorio.Delete(id);
            if (!resp)
            {
                return BadRequest("La venta no se pudo borrar");
            }
            return Ok();
        }

        // PUT: api/Ventas/{id}/estado
        [HttpPut("{id:int}/estado")]
        public async Task<ActionResult> ActualizarEstado(int id, [FromBody] string estado)
        {
            try
            {
                var resultado = await repositorio.ActualizarEstado(id, estado);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el estado");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/Ventas/{id}/registrar-pago
        [HttpPost("{id:int}/registrar-pago")]
        public async Task<ActionResult> RegistrarPago(int id, [FromBody] RegistrarPagoRequest request)
        {
            try
            {
                var resultado = await repositorio.RegistrarPago(id, request.FechaPago, request.NumeroComprobante);
                if (!resultado)
                {
                    return BadRequest("No se pudo registrar el pago");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/Ventas/{id}/registrar-envio
        [HttpPost("{id:int}/registrar-envio")]
        public async Task<ActionResult> RegistrarEnvio(int id, [FromBody] RegistrarEnvioRequest request)
        {
            try
            {
                var resultado = await repositorio.RegistrarEnvio(id, request.NumeroTracking, request.UrlTracking);
                if (!resultado)
                {
                    return BadRequest("No se pudo registrar el envío");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/Ventas/crear-con-detalles (CHECKOUT)
        [HttpPost("crear-con-detalles")]
        public async Task<ActionResult<int>> CrearVentaConDetalles([FromBody] CrearVentaRequest request)
        {
            try
            {
                // Crear la venta a partir del DTO
                var venta = new Venta
                {
                    Fecha = request.Fecha != default ? request.Fecha : DateTime.Now,
                    NombreCliente = request.NombreCliente,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    TipoEnvio = request.TipoEnvio,
                    DireccionEnvio = request.DireccionEnvio,
                    CiudadEnvio = request.CiudadEnvio,
                    CodigoPostalEnvio = request.CodigoPostalEnvio,
                    ProvinciaEnvio = request.ProvinciaEnvio,
                    MetodoPago = request.MetodoPago,
                    NumeroComprobante = request.NumeroComprobante,
                    FechaPago = request.FechaTransferencia,
                    BancoOrigen = request.BancoOrigen,
                    TitularCuenta = request.TitularCuenta,
                    Notas = request.Notas,
                    Subtotal = request.Subtotal,
                    CostoEnvio = request.CostoEnvio,
                    Total = request.Total,
                    Estado = request.MetodoPago == "Transferencia" ? "EsperandoPago" : "Pendiente",
                    Detalles = request.Detalles.Select(d => new VentaDetalle
                    {
                        ProductoId = d.ProductoId,
                        Color = d.Color ?? "",
                        Tamanio = d.Tamanio ?? "",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    }).ToList()
                };

                return await repositorio.Insert(venta);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }
    }

    // =============================================
    // CLASES AUXILIARES PARA REQUESTS
    // =============================================

    public class CrearVentaRequest
    {
        // Datos del cliente
        public string NombreCliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Envío
        public string TipoEnvio { get; set; } = "RetiroLocal";
        public string? DireccionEnvio { get; set; }
        public string? CiudadEnvio { get; set; }
        public string? CodigoPostalEnvio { get; set; }
        public string? ProvinciaEnvio { get; set; }

        // Pago
        public string MetodoPago { get; set; } = "Pendiente";
        public string? NumeroComprobante { get; set; }
        public DateTime? FechaTransferencia { get; set; }
        public string? BancoOrigen { get; set; }
        public string? TitularCuenta { get; set; }

        // Observaciones
        public string? Notas { get; set; }

        // Detalles de productos
        public List<CrearVentaDetalleRequest> Detalles { get; set; } = new List<CrearVentaDetalleRequest>();

        // Totales
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }

        // Fecha (opcional)
        public DateTime Fecha { get; set; }
    }

    public class CrearVentaDetalleRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Color { get; set; }
        public string? Tamanio { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class RegistrarPagoRequest
    {
        public DateTime FechaPago { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
    }

    public class RegistrarEnvioRequest
    {
        public string NumeroTracking { get; set; } = string.Empty;
        public string UrlTracking { get; set; } = string.Empty;
    }
}