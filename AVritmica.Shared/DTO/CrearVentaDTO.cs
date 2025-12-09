using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.Shared.DTO
{
    public class CrearVentaDTO
    {
        // DATOS DEL CLIENTE (obligatorios)
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

        // TIPO DE ENVÍO (obligatorio)
        [Required(ErrorMessage = "Debe seleccionar un tipo de envío")]
        [RegularExpression("^(RetiroLocal|EnvioDomicilio)$", ErrorMessage = "Tipo de envío inválido")]
        public string TipoEnvio { get; set; } = "RetiroLocal";

        // DATOS DE ENVÍO (obligatorios solo si es EnvioDomicilio)
        [MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? DireccionEnvio { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? CiudadEnvio { get; set; }

        [MaxLength(10, ErrorMessage = "Máximo 10 caracteres")]
        public string? CodigoPostalEnvio { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? ProvinciaEnvio { get; set; }

        // MÉTODO DE PAGO (obligatorio)
        [Required(ErrorMessage = "Debe seleccionar un método de pago")]
        [RegularExpression("^(EfectivoLocal|Transferencia)$", ErrorMessage = "Método de pago inválido")]
        public string MetodoPago { get; set; } = "Pendiente";

        // DATOS DE TRANSFERENCIA (obligatorios solo si es Transferencia)
        [MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string? NumeroComprobante { get; set; }

        public DateTime? FechaTransferencia { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? BancoOrigen { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? TitularCuenta { get; set; }

        // OBSERVACIONES
        [MaxLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string? Notas { get; set; }

        // DETALLES DE PRODUCTOS (obligatorios)
        [Required(ErrorMessage = "Debe agregar al menos un producto")]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto")]
        public List<CrearVentaDetalleDTO> Detalles { get; set; } = new();

        // TOTALES (calculados en frontend, validados en backend)
        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "El costo de envío es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo de envío no puede ser negativo")]
        public decimal CostoEnvio { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        // MÉTODO DE VALIDACIÓN PARA ENVÍOS
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoEnvio == "EnvioDomicilio")
            {
                if (string.IsNullOrEmpty(DireccionEnvio))
                    yield return new ValidationResult("La dirección es obligatoria para envíos a domicilio", new[] { nameof(DireccionEnvio) });

                if (string.IsNullOrEmpty(CodigoPostalEnvio))
                    yield return new ValidationResult("El código postal es obligatorio para envíos a domicilio", new[] { nameof(CodigoPostalEnvio) });

                if (string.IsNullOrEmpty(CiudadEnvio))
                    yield return new ValidationResult("La ciudad es obligatoria para envíos a domicilio", new[] { nameof(CiudadEnvio) });
            }

            if (MetodoPago == "Transferencia")
            {
                if (string.IsNullOrEmpty(NumeroComprobante))
                    yield return new ValidationResult("El número de comprobante es obligatorio para transferencias", new[] { nameof(NumeroComprobante) });
            }

            // Validar que total = subtotal + costoEnvio
            if (Total != Subtotal + CostoEnvio)
            {
                yield return new ValidationResult($"El total ({Total}) no coincide con subtotal ({Subtotal}) + envío ({CostoEnvio})", new[] { nameof(Total) });
            }
        }
    }
}