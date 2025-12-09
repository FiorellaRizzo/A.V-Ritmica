using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.BD.Data.Entity
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de venta es obligatorio")]
        [MaxLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string NumeroVenta { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Pagada, Enviada, Completada, Cancelada

        // DATOS DEL CLIENTE
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        // TIPO DE ENVÍO
        [Required]
        [MaxLength(20)]
        public string TipoEnvio { get; set; } = "RetiroLocal"; // "RetiroLocal" o "EnvioDomicilio"

        // SOLO si es EnvioDomicilio
        [MaxLength(200)]
        public string? DireccionEnvio { get; set; }

        [MaxLength(100)]
        public string? CiudadEnvio { get; set; }

        [MaxLength(10)]
        public string? CodigoPostalEnvio { get; set; }

        [MaxLength(100)]
        public string? ProvinciaEnvio { get; set; }

        // COSTOS
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoEnvio { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // MÉTODO DE PAGO
        [Required]
        [MaxLength(20)]
        public string MetodoPago { get; set; } = "Pendiente"; // "EfectivoLocal", "Transferencia", "Pendiente"

        // DATOS DE PAGO (si transferencia)
        [MaxLength(50)]
        public string? NumeroComprobante { get; set; }

        public DateTime? FechaPago { get; set; }

        [MaxLength(100)]
        public string? BancoOrigen { get; set; }

        [MaxLength(100)]
        public string? TitularCuenta { get; set; }

        // OBSERVACIONES
        [MaxLength(500)]
        public string? Notas { get; set; }

        // SEGUIMIENTO ANDREANI (para futuro)
        [MaxLength(50)]
        public string? NumeroTracking { get; set; }

        [MaxLength(500)]
        public string? UrlTracking { get; set; }

        // RELACIONES
        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();

        // MÉTODO PARA GENERAR NÚMERO DE VENTA
        public void GenerarNumeroVenta(int ultimoNumero)
        {
            NumeroVenta = $"VTA-{(ultimoNumero + 1):0000}";
        }

        // MÉTODO PARA CALCULAR TOTAL
        public void CalcularTotales()
        {
            Subtotal = Detalles.Sum(d => d.Subtotal);
            Total = Subtotal + CostoEnvio;
        }
    }
}