using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.Shared.DTO
{
    public class VentaDTO
    {
        public int Id { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;

        // Datos del cliente
        public string NombreCliente { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Envío
        public string TipoEnvio { get; set; } = string.Empty;
        public string? DireccionEnvio { get; set; }
        public string? CiudadEnvio { get; set; }
        public string? CodigoPostalEnvio { get; set; }
        public string? ProvinciaEnvio { get; set; }

        // Totales
        public decimal Subtotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }

        // Pago
        public string MetodoPago { get; set; } = string.Empty;
        public string? NumeroComprobante { get; set; }
        public DateTime? FechaPago { get; set; }

        // Detalles
        public List<VentaDetalleDTO> Detalles { get; set; } = new();

        // Tracking (opcional)
        public string? NumeroTracking { get; set; }
        public string? UrlTracking { get; set; }

        // Método para mostrar resumen
        public string Resumen =>
            $"{NumeroVenta} - {NombreCliente} - ${Total} - {Estado}";
    }
}
