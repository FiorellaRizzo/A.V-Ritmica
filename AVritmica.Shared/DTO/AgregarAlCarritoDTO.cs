
using System.ComponentModel.DataAnnotations;

namespace AVritmica.Shared.DTO
{
    public class AgregarAlCarritoDTO
    {
        // ========== DATOS DEL PRODUCTO ==========
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        public int Cantidad { get; set; } = 1;

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        // ========== VARIANTES ==========
        [MaxLength(100, ErrorMessage = "El color no puede exceder 100 caracteres")]
        public string? Color { get; set; }

        [MaxLength(100, ErrorMessage = "El tamaño no puede exceder 100 caracteres")]
        public string? Tamaño { get; set; }

        // ========== USUARIO ==========
        public int UsuarioId { get; set; } = 0;

        // ========== MÉTODOS HELPER ==========
        public bool EsInvitado => UsuarioId <= 0;

        public string GetClaveUnica()
        {
            if (string.IsNullOrEmpty(Color) && string.IsNullOrEmpty(Tamaño))
                return ProductoId.ToString();

            return $"{ProductoId}_{Color ?? ""}_{Tamaño ?? ""}";
        }
    }
}