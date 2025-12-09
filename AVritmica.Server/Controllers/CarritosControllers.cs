using Microsoft.AspNetCore.Mvc;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;
using AVritmica.Shared.DTO;
using System.Text.Json;

namespace AVritmica.Server.Controllers
{
    [ApiController]
    [Route("api/Carritos")]
    public class CarritosController : ControllerBase
    {
        private readonly ICarritoRepositorio repositorio;

        public CarritosController(ICarritoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        //  ENDPOINTS EXISTENTES 

        [HttpGet]    // api/Carritos
        public async Task<ActionResult<List<Carrito>>> Get()
        {
            return await repositorio.Select();
        }

        [HttpGet("{id:int}")] // api/Carritos/2
        public async Task<ActionResult<Carrito>> Get(int id)
        {
            Carrito? carrito = await repositorio.SelectById(id);
            if (carrito == null)
            {
                return NotFound();
            }
            return carrito;
        }

        [HttpGet("GetByUsuario/{usuarioId:int}")] // api/Carritos/GetByUsuario/1
        public async Task<ActionResult<List<Carrito>>> GetByUsuario(int usuarioId)
        {
            var carritos = await repositorio.SelectByUsuario(usuarioId);
            return carritos;
        }

        [HttpGet("GetByEstado/{estado}")] // api/Carritos/GetByEstado/Activo
        public async Task<ActionResult<List<Carrito>>> GetByEstado(string estado)
        {
            var carritos = await repositorio.SelectByEstado(estado);
            return carritos;
        }

        [HttpGet("GetByEstadoPago/{estadoPago}")] // api/Carritos/GetByEstadoPago/Pendiente
        public async Task<ActionResult<List<Carrito>>> GetByEstadoPago(string estadoPago)
        {
            var carritos = await repositorio.SelectByEstadoPago(estadoPago);
            return carritos;
        }

        [HttpGet("GetCarritoActivo/{usuarioId:int}")] // api/Carritos/GetCarritoActivo/1
        public async Task<ActionResult<Carrito>> GetCarritoActivo(int usuarioId)
        {
            Carrito? carrito = await repositorio.SelectCarritoActivoByUsuario(usuarioId);
            if (carrito == null)
            {
                return NotFound();
            }
            return carrito;
        }

        [HttpGet("existe/{id:int}")] // api/Carritos/existe/2
        public async Task<ActionResult<bool>> Existe(int id)
        {
            return await repositorio.Existe(id);
        }

        [HttpGet("existeCarritoActivo/{usuarioId:int}")] // api/Carritos/existeCarritoActivo/1
        public async Task<ActionResult<bool>> ExisteCarritoActivo(int usuarioId)
        {
            return await repositorio.ExisteCarritoActivo(usuarioId);
        }

        [HttpPost("actualizar-estado/{id:int}")] // api/Carritos/actualizar-estado/2
        public async Task<ActionResult> ActualizarEstado(int id, [FromBody] string estado)
        {
            try
            {
                var resultado = await repositorio.ActualizarEstado(id, estado);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el estado del carrito");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("actualizar-estado-pago/{id:int}")] // api/Carritos/actualizar-estado-pago/2
        public async Task<ActionResult> ActualizarEstadoPago(int id, [FromBody] string estadoPago)
        {
            try
            {
                var resultado = await repositorio.ActualizarEstadoPago(id, estadoPago);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el estado de pago del carrito");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("confirmar-carrito/{id:int}")] // api/Carritos/confirmar-carrito/2
        public async Task<ActionResult> ConfirmarCarrito(int id, [FromBody] ConfirmarCarritoRequest request)
        {
            try
            {
                var resultado = await repositorio.ConfirmarCarrito(id, request.MontoTotal, request.DireccionEnvio);
                if (!resultado)
                {
                    return BadRequest("No se pudo confirmar el carrito");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("actualizar-monto/{id:int}")] // api/Carritos/actualizar-monto/2
        public async Task<ActionResult> ActualizarMontoTotal(int id, [FromBody] decimal montoTotal)
        {
            try
            {
                var resultado = await repositorio.ActualizarMontoTotal(id, montoTotal);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el monto total del carrito");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Método POST original mantenido para compatibilidad
        [HttpPost]
        public async Task<ActionResult<int>> Post(Carrito entidad)
        {
            try
            {
                // Verificar si el usuario ya tiene un carrito activo
                if (entidad.Estado == "Activo" && await repositorio.ExisteCarritoActivo(entidad.UsuarioId))
                {
                    return BadRequest("El usuario ya tiene un carrito activo");
                }

                return await repositorio.Insert(entidad);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        // Nuevo método POST que acepta DTO
        [HttpPost("crear")]
        public async Task<ActionResult<int>> CrearCarrito([FromBody] CarritoCreateRequest request)
        {
            try
            {
                // Verificar si el usuario ya tiene un carrito activo
                if (request.Estado == "Activo" && await repositorio.ExisteCarritoActivo(request.UsuarioId))
                {
                    return BadRequest("El usuario ya tiene un carrito activo");
                }

                var carrito = new Carrito
                {
                    UsuarioId = request.UsuarioId,
                    Estado = request.Estado,
                    EstadoPago = request.EstadoPago,
                    MontoTotal = request.MontoTotal,
                    Saldo = request.Saldo,
                    DireccionEnvio = request.DireccionEnvio,
                    FechaCreacion = DateTime.UtcNow
                };

                return await repositorio.Insert(carrito);
            }
            catch (Exception err)
            {
                return BadRequest(err.Message);
            }
        }

        // Método PUT original mantenido para compatibilidad
        [HttpPut("{id:int}")] // api/Carritos/2
        public async Task<ActionResult> Put(int id, [FromBody] Carrito entidad)
        {
            try
            {
                if (id != entidad.Id)
                {
                    return BadRequest("Datos Incorrectos");
                }

                // Validar que no se active otro carrito si ya existe uno activo
                if (entidad.Estado == "Activo")
                {
                    var carritoExistente = await repositorio.SelectCarritoActivoByUsuario(entidad.UsuarioId);
                    if (carritoExistente != null && carritoExistente.Id != id)
                    {
                        return BadRequest("El usuario ya tiene otro carrito activo");
                    }
                }

                var resultado = await repositorio.Update(id, entidad);

                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el carrito");
                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Nuevo método PUT que acepta DTO
        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> ActualizarCarrito(int id, [FromBody] CarritoUpdateRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("Datos Incorrectos");
                }

                var carritoExistente = await repositorio.SelectById(id);
                if (carritoExistente == null)
                {
                    return NotFound();
                }

                // Validar que no se active otro carrito si ya existe uno activo
                if (request.Estado == "Activo")
                {
                    var carritoActivo = await repositorio.SelectCarritoActivoByUsuario(request.UsuarioId);
                    if (carritoActivo != null && carritoActivo.Id != id)
                    {
                        return BadRequest("El usuario ya tiene otro carrito activo");
                    }
                }

                // Actualizar propiedades
                carritoExistente.UsuarioId = request.UsuarioId;
                carritoExistente.Estado = request.Estado;
                carritoExistente.EstadoPago = request.EstadoPago;
                carritoExistente.MontoTotal = request.MontoTotal;
                carritoExistente.Saldo = request.Saldo;
                carritoExistente.DireccionEnvio = request.DireccionEnvio;

                var resultado = await repositorio.Update(id, carritoExistente);

                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el carrito");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id:int}")] // api/Carritos/2
        public async Task<ActionResult> Delete(int id)
        {
            var resp = await repositorio.Delete(id);
            if (!resp)
            {
                return BadRequest("El carrito no se pudo borrar");
            }
            return Ok();
        }

        // NUEVOS ENDPOINTS PARA EL CARRITO DE COMPRAS

        [HttpPost("AgregarItem")]
        public async Task<ActionResult> AgregarItem([FromBody] AgregarAlCarritoDTO itemDTO)
        {
            try
            {
                // Log para debugging
                Console.WriteLine($" AgregarItem recibido: {JsonSerializer.Serialize(itemDTO)}");

                //  Validar datos básicos
                if (itemDTO.UsuarioId <= 0)
                {
                    return BadRequest(new { Success = false, Message = "UsuarioId es requerido" });
                }

                if (itemDTO.Cantidad <= 0)
                {
                    return BadRequest(new { Success = false, Message = "La cantidad debe ser mayor a 0" });
                }

                if (itemDTO.PrecioUnitario <= 0)
                {
                    return BadRequest(new { Success = false, Message = "El precio debe ser mayor a 0" });
                }

                // Obtener o crear carrito activo
                var carrito = await repositorio.SelectCarritoActivoByUsuario(itemDTO.UsuarioId);

                if (carrito == null)
                {
                    Console.WriteLine($"🛒 Creando nuevo carrito para usuario {itemDTO.UsuarioId}");

                    // Crear nuevo carrito
                    carrito = new Carrito
                    {
                        UsuarioId = itemDTO.UsuarioId,
                        Estado = "Activo",
                        EstadoPago = "Pendiente",
                        FechaCreacion = DateTime.UtcNow,
                        MontoTotal = 0,
                        Saldo = 0,
                        DireccionEnvio = ""
                    };

                    var carritoId = await repositorio.Insert(carrito);
                    carrito.Id = carritoId;
                    Console.WriteLine($"✅ Carrito creado con ID: {carritoId}");
                }
                else
                {
                    Console.WriteLine($"✅ Carrito existente encontrado ID: {carrito.Id}");
                }

                //  Verificar si el producto ya está en el carrito (con las mismas variantes)
                var itemExistente = carrito.CarritoProductos?.FirstOrDefault(cp =>
                    cp.ProductoId == itemDTO.ProductoId &&
                    cp.Color == itemDTO.Color &&
                    cp.Tamaño == itemDTO.Tamaño);

                if (itemExistente != null)
                {
                    // Actualizar cantidad si ya existe
                    itemExistente.Cantidad += itemDTO.Cantidad;
                    Console.WriteLine($"🔄 Producto existente, actualizando cantidad a: {itemExistente.Cantidad}");
                }
                else
                {
                    // Agregar nuevo item al carrito
                    carrito.CarritoProductos ??= new List<CarritoProducto>();
                    carrito.CarritoProductos.Add(new CarritoProducto
                    {
                        ProductoId = itemDTO.ProductoId,
                        Cantidad = itemDTO.Cantidad,
                        PrecioUnitario = itemDTO.PrecioUnitario,
                        Color = itemDTO.Color,
                        Tamaño = itemDTO.Tamaño
                    });
                    Console.WriteLine($"➕ Nuevo producto agregado al carrito");
                }

                //  Recalcular total del carrito
                carrito.MontoTotal = carrito.CarritoProductos?.Sum(cp => cp.Cantidad * cp.PrecioUnitario) ?? 0;

                //  Actualizar carrito en BD
                var resultadoUpdate = await repositorio.Update(carrito.Id, carrito);

                if (!resultadoUpdate)
                {
                    return BadRequest(new { Success = false, Message = "No se pudo actualizar el carrito" });
                }

                // 6. Obtener carrito actualizado para la respuesta
                carrito = await repositorio.SelectCarritoActivoByUsuario(itemDTO.UsuarioId);
                var totalItems = carrito?.CarritoProductos?.Sum(cp => cp.Cantidad) ?? 0;

                Console.WriteLine($" Producto agregado exitosamente. Total items: {totalItems}, Monto total: {carrito?.MontoTotal ?? 0}");

                return Ok(new
                {
                    Success = true,
                    CarritoId = carrito?.Id ?? 0,
                    TotalItems = totalItems,
                    MontoTotal = carrito?.MontoTotal ?? 0,
                    Message = "Producto agregado al carrito exitosamente"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error en AgregarItem: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return BadRequest(new
                {
                    Success = false,
                    Message = $"Error al agregar producto: {ex.Message}"
                });
            }
        }

        [HttpGet("items")]
        public async Task<ActionResult<List<CarritoItemDTO>>> ObtenerItemsCarrito([FromQuery] int usuarioId)
        {
            try
            {
                Console.WriteLine($"ObtenerItemsCarrito para usuario: {usuarioId}");

                if (usuarioId <= 0)
                {
                    return BadRequest("UsuarioId es requerido");
                }

                var carrito = await repositorio.SelectCarritoActivoByUsuario(usuarioId);
                if (carrito == null || carrito.CarritoProductos == null || !carrito.CarritoProductos.Any())
                {
                    Console.WriteLine($" Carrito vacío o no encontrado para usuario: {usuarioId}");
                    return new List<CarritoItemDTO>();
                }

                Console.WriteLine($" Carrito encontrado con {carrito.CarritoProductos.Count} productos");

                // Convertir a DTOs
                var items = carrito.CarritoProductos.Select(cp => new CarritoItemDTO
                {
                    Id = cp.Id,
                    ProductoId = cp.ProductoId,
                    ProductoNombre = cp.Producto?.Nombre ?? "Producto",
                    ProductoImagen = cp.Producto?.ImagenUrl ?? "",
                    ProductoDescripcion = cp.Producto?.Descripcion ?? "",
                    Cantidad = cp.Cantidad,
                    PrecioUnitario = cp.PrecioUnitario,
                    Color = cp.Color,
                    Tamaño = cp.Tamaño,
                    CarritoId = carrito.Id,
                    CategoriaId = cp.Producto?.CategoriaId ?? 0,
                    CategoriaNombre = cp.Producto?.Categoria?.Nombre ?? "",
                    StockDisponible = cp.Producto?.Stock ?? 0,
                    TieneVariantes = !string.IsNullOrEmpty(cp.Producto?.ColoresDisponibles),
                    // se usa DateTime.UtcNow como fecha de agregado si no hay propiedad específica
                    FechaAgregado = DateTime.UtcNow
                }).ToList();

                Console.WriteLine($" Convertidos {items.Count} items a DTOs");
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error en ObtenerItemsCarrito: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("actualizar-cantidad")]
        public async Task<ActionResult> ActualizarCantidadItem([FromBody] ActualizarCantidadRequest request)
        {
            try
            {
                Console.WriteLine($"🔄 ActualizarCantidadItem: {JsonSerializer.Serialize(request)}");

                if (request.UsuarioId <= 0)
                {
                    return BadRequest(new { Success = false, Message = "UsuarioId es requerido" });
                }

                if (request.Cantidad < 0)
                {
                    return BadRequest(new { Success = false, Message = "La cantidad no puede ser negativa" });
                }

                var carrito = await repositorio.SelectCarritoActivoByUsuario(request.UsuarioId);
                if (carrito == null)
                {
                    Console.WriteLine($"❌ Carrito no encontrado para usuario: {request.UsuarioId}");
                    return NotFound(new { Success = false, Message = "Carrito no encontrado" });
                }

                Console.WriteLine($"✅ Carrito encontrado ID: {carrito.Id}");

                
                var resultado = await repositorio.ActualizarCantidadProducto(
                    carrito.Id,
                    request.ProductoId,
                    request.Cantidad,
                    request.Color,
                    request.Tamaño
                );

                if (!resultado)
                {
                    return BadRequest(new { Success = false, Message = "No se pudo actualizar la cantidad" });
                }

                Console.WriteLine($"✅ Cantidad actualizada exitosamente");
                return Ok(new { Success = true, Message = "Cantidad actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ActualizarCantidadItem: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("eliminar-item/{productoId}")]
        public async Task<ActionResult> EliminarItem(
        int productoId,
        [FromQuery] int usuarioId,
        [FromQuery] string? color = null,
        [FromQuery] string? tamaño = null)
        {
            try
            {
                Console.WriteLine($" EliminarItem: ProductoId={productoId}, UsuarioId={usuarioId}, Color={color}, Tamaño={tamaño}");

                if (usuarioId <= 0)
                {
                    return BadRequest(new { Success = false, Message = "UsuarioId es requerido" });
                }

                var carrito = await repositorio.SelectCarritoActivoByUsuario(usuarioId);
                if (carrito == null)
                {
                    Console.WriteLine($" Carrito no encontrado para usuario: {usuarioId}");
                    return NotFound(new { Success = false, Message = "Carrito no encontrado" });
                }

                Console.WriteLine($" Carrito encontrado ID: {carrito.Id}");

                
                var resultado = await repositorio.EliminarProductoDelCarrito(
                    carrito.Id,
                    productoId,
                    color,
                    tamaño
                );

                if (!resultado)
                {
                    return BadRequest(new { Success = false, Message = "No se pudo eliminar el producto del carrito" });
                }

                Console.WriteLine($"Producto eliminado del carrito");
                return Ok(new { Success = true, Message = "Producto eliminado del carrito exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error en EliminarItem: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("vaciar")]
        public async Task<ActionResult> VaciarCarrito([FromQuery] int usuarioId)
        {
            try
            {
                Console.WriteLine($"🧹 VaciarCarrito para usuario: {usuarioId}");

                if (usuarioId <= 0)
                {
                    return BadRequest(new { Success = false, Message = "UsuarioId es requerido" });
                }

                var carrito = await repositorio.SelectCarritoActivoByUsuario(usuarioId);
                if (carrito == null)
                {
                    Console.WriteLine($" Carrito no encontrado para usuario: {usuarioId}");
                    return NotFound(new { Success = false, Message = "Carrito no encontrado" });
                }

                Console.WriteLine($" Carrito encontrado ID: {carrito.Id}");

                
                var resultado = await repositorio.VaciarCarrito(carrito.Id);

                if (!resultado)
                {
                    return BadRequest(new { Success = false, Message = "No se pudo vaciar el carrito" });
                }

                Console.WriteLine($" Carrito vaciado exitosamente");
                return Ok(new { Success = true, Message = "Carrito vaciado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error en VaciarCarrito: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<CarritoResumenDTO>> ObtenerResumen([FromQuery] int usuarioId)
        {
            try
            {
                Console.WriteLine($" ObtenerResumen para usuario: {usuarioId}");

                if (usuarioId <= 0)
                {
                    return BadRequest("UsuarioId es requerido");
                }

                var carrito = await repositorio.SelectCarritoActivoByUsuario(usuarioId);

                var resumen = new CarritoResumenDTO
                {
                    UsuarioId = usuarioId,
                    CarritoId = carrito?.Id ?? 0,
                    Estado = carrito?.Estado ?? "Sin carrito",
                    FechaCreacion = carrito?.FechaCreacion ?? DateTime.UtcNow
                };

                if (carrito != null && carrito.CarritoProductos != null && carrito.CarritoProductos.Any())
                {
                    Console.WriteLine($"Carrito encontrado con {carrito.CarritoProductos.Count} productos");

                    // Convertir items a DTOs
                    var items = carrito.CarritoProductos.Select(cp => new CarritoItemDTO
                    {
                        ProductoId = cp.ProductoId,
                        ProductoNombre = cp.Producto?.Nombre ?? "Producto",
                        Cantidad = cp.Cantidad,
                        PrecioUnitario = cp.PrecioUnitario,
                        Color = cp.Color,
                        Tamaño = cp.Tamaño,
                        CarritoId = carrito.Id
                    }).ToList();

                    resumen.CalcularDesdeItems(items);
                    Console.WriteLine($" Resumen calculado: {resumen.ObtenerResumenTexto()}");
                }
                else
                {
                    Console.WriteLine($" Carrito vacío para usuario: {usuarioId}");
                }

                return resumen;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error en ObtenerResumen: {ex.Message}");
                Console.WriteLine($" StackTrace: {ex.StackTrace}");
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }

    // CLASES AUXILIARES

    // Clase auxiliar para el request de confirmar carrito
    public class ConfirmarCarritoRequest
    {
        public decimal MontoTotal { get; set; }
        public string DireccionEnvio { get; set; } = string.Empty;
    }

    // Clases DTO para crear y actualizar carritos
    public class CarritoCreateRequest
    {
        public int UsuarioId { get; set; }
        public string Estado { get; set; } = "Activo";
        public string EstadoPago { get; set; } = "Pendiente";
        public decimal MontoTotal { get; set; }
        public decimal Saldo { get; set; }
        public string DireccionEnvio { get; set; } = string.Empty;
    }

    public class CarritoUpdateRequest
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Estado { get; set; } = "Activo";
        public string EstadoPago { get; set; } = "Pendiente";
        public decimal MontoTotal { get; set; }
        public decimal Saldo { get; set; }
        public string DireccionEnvio { get; set; } = string.Empty;
    }

    // Clase auxiliar para actualizar cantidad
    public class ActualizarCantidadRequest
    {
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Color { get; set; }
        public string? Tamaño { get; set; }
    }
}