using Microsoft.AspNetCore.Mvc;
using AVritmica.BD.Data.Entity;
using AVritmica.Server.Repositorio;
using AVritmica.Shared.DTO;

namespace AVritmica.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepositorio _repositorio;

        public ProductosController(IProductoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> Get()
        {
            try
            {
                var productos = await _repositorio.Select();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> Get(int id)
        {
            try
            {
                var producto = await _repositorio.SelectById(id);
                if (producto == null)
                {
                    return NotFound($"Producto con ID {id} no encontrado");
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("GetByNombre/{nombre}")]
        public async Task<ActionResult<Producto>> GetByNombre(string nombre)
        {
            try
            {
                var producto = await _repositorio.SelectByNombre(nombre);
                if (producto == null)
                {
                    return NotFound($"Producto con nombre '{nombre}' no encontrado");
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("GetByCategoria/{categoriaId:int}")]
        public async Task<ActionResult<List<Producto>>> GetByCategoria(int categoriaId)
        {
            try
            {
                var productos = await _repositorio.SelectByCategoria(categoriaId);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("GetByPrecioRange")]
        public async Task<ActionResult<List<Producto>>> GetByPrecioRange([FromQuery] decimal precioMin, [FromQuery] decimal precioMax)
        {
            try
            {
                if (precioMin < 0 || precioMax < 0)
                {
                    return BadRequest("Los precios no pueden ser negativos");
                }

                if (precioMin > precioMax)
                {
                    return BadRequest("El precio mínimo no puede ser mayor al precio máximo");
                }

                var productos = await _repositorio.SelectByPrecioRange(precioMin, precioMax);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("existe/{id:int}")]
        public async Task<ActionResult<bool>> Existe(int id)
        {
            try
            {
                var existe = await _repositorio.Existe(id);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("existeNombre/{nombre}")]
        public async Task<ActionResult<bool>> ExisteNombre(string nombre)
        {
            try
            {
                var existe = await _repositorio.Existe(nombre);
                return Ok(existe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("actualizar-stock/{id:int}")]
        public async Task<ActionResult> ActualizarStock(int id, [FromBody] int cantidad)
        {
            try
            {
                // Verificar si el producto existe
                if (!await _repositorio.Existe(id))
                {
                    return NotFound($"Producto con ID {id} no encontrado");
                }

                var resultado = await _repositorio.ActualizarStock(id, cantidad);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el stock del producto");
                }

                return Ok("Stock actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el stock: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> Post(CrearProductoDTO productoDTO)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si ya existe un producto con el mismo nombre
                if (await _repositorio.Existe(productoDTO.Nombre))
                {
                    return BadRequest("Ya existe un producto con ese nombre");
                }

                var producto = new Producto
                {
                    Nombre = productoDTO.Nombre,
                    Descripcion = productoDTO.Descripcion,
                    Precio = productoDTO.Precio,
                    Stock = productoDTO.Stock,
                    ImagenUrl = productoDTO.ImagenUrl,
                    CategoriaId = productoDTO.CategoriaId,
                    // CAMPOS PARA VARIANTES
                    TieneVariantes = productoDTO.TieneVariantes,
                    ColoresDisponibles = productoDTO.ColoresDisponibles ?? "",
                    TamaniosDisponibles = productoDTO.TamaniosDisponibles ?? "",
                    ImagenesVariantes = productoDTO.ImagenesVariantes ?? "",
                    // AGREGAR StockPorColor
                    StockPorColor = productoDTO.StockPorColor ?? ""
                };

                var productoId = await _repositorio.Insert(producto);
                producto.Id = productoId;

                // Recargar el producto con la categoría incluida
                var productoCompleto = await _repositorio.SelectById(productoId);
                return CreatedAtAction(nameof(Get), new { id = productoId }, productoCompleto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el producto: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Producto entidad)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != entidad.Id)
                {
                    return BadRequest("ID del producto no coincide");
                }

                // Verificar si el producto existe
                if (!await _repositorio.Existe(id))
                {
                    return NotFound($"Producto con ID {id} no encontrado");
                }

                // Verificar si el nombre ya existe en otro producto
                var productoExistente = await _repositorio.SelectByNombre(entidad.Nombre);
                if (productoExistente != null && productoExistente.Id != id)
                {
                    return BadRequest("Ya existe otro producto con ese nombre");
                }

                var resultado = await _repositorio.Update(id, entidad);
                if (!resultado)
                {
                    return BadRequest("No se pudo actualizar el producto");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar el producto: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Verificar si el producto existe
                if (!await _repositorio.Existe(id))
                {
                    return NotFound($"Producto con ID {id} no encontrado");
                }

                var resultado = await _repositorio.Delete(id);
                if (!resultado)
                {
                    return BadRequest("No se pudo eliminar el producto");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el producto: {ex.Message}");
            }
        }
    }
}
