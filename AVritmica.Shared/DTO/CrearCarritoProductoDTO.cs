using System.ComponentModel.DataAnnotations;

namespace AVritmica.Shared.DTO
{
    public class CrearCarritoProductoDTO
    {
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El carrito es requerido")]
        public int CarritoId { get; set; }

        // NUEVOS CAMPOS PARA VARIANTES
        [MaxLength(100, ErrorMessage = "El color no puede exceder 100 caracteres")]
        public string? Color { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño no puede exceder 100 caracteres")]
        public string? Tamaño { get; set; }
    }
}