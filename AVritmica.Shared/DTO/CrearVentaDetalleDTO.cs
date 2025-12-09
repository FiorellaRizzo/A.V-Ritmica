using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.Shared.DTO
{
    public class CrearVentaDetalleDTO
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        // Para variantes (si el producto las tiene)
        public string Color { get; set; } = string.Empty;
        public string Tamanio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        // Propiedad calculada (opcional, pero útil)
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
