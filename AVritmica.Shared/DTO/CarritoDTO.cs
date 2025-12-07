using System.ComponentModel.DataAnnotations;

namespace AVritmica.Shared.DTO
{
    public class CarritoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        public int UsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(50)]
        public string Estado { get; set; } = "Activo";

        public DateTime? FechaConfirmacion { get; set; }

        [Required(ErrorMessage = "El estado de pago es requerido")]
        [StringLength(20)]
        public string EstadoPago { get; set; } = "Pendiente";

        [Range(0, double.MaxValue, ErrorMessage = "El monto total debe ser mayor o igual a 0")]
        public decimal MontoTotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El saldo debe ser mayor o igual a 0")]
        public decimal Saldo { get; set; }

        [StringLength(500)]
        public string DireccionEnvio { get; set; } = string.Empty;

      
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
