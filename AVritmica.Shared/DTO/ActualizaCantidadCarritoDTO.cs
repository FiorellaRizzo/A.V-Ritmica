// AVritmica.Shared/DTO/ActualizarCantidadDTO.cs
using System.ComponentModel.DataAnnotations;

namespace AVritmica.Shared.DTO
{
    public class ActualizarCantidadDTO
    {
        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, 100, ErrorMessage = "La cantidad debe estar entre 0 y 100")]
        public int Cantidad { get; set; }

        // Variantes
        public string? Color { get; set; }
        public string? Tamaño { get; set; }

        // Para identificar el carrito
        public int CarritoId { get; set; }
        public int UsuarioId { get; set; }

        // Método para validar si es eliminar (cantidad = 0)
        public bool EsEliminar => Cantidad <= 0;
    }
}