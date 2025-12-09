using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.Shared.DTO
{
    public class VentaDetalleDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string? ProductoImagenUrl { get; set; }

        // Variantes
        public string Color { get; set; } = string.Empty;
        public string Tamanio { get; set; } = string.Empty;

        // Cantidad y precios
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        // Método para mostrar variante
        public string VarianteInfo
        {
            get
            {
                var info = new List<string>();
                if (!string.IsNullOrEmpty(Color)) info.Add($"Color: {Color}");
                if (!string.IsNullOrEmpty(Tamanio)) info.Add($"Tamaño: {Tamanio}");
                return string.Join(" | ", info);
            }
        }

        // Método para mostrar resumen
        public string Resumen =>
            $"{Cantidad} x {ProductoNombre} - ${PrecioUnitario} c/u";
    }
}