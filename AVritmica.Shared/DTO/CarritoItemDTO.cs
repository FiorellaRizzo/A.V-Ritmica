
namespace AVritmica.Shared.DTO
{
    public class CarritoItemDTO
    {
        // Información del item
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoImagen { get; set; } = string.Empty;
        public string ProductoDescripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;

        // Información de compra
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        // Variantes
        public string? Color { get; set; }
        public string? Tamaño { get; set; }

        // Stock y validación
        public int StockDisponible { get; set; }
        public bool TieneVariantes { get; set; }
        public bool TieneStockSuficiente => Cantidad <= StockDisponible;

        // Para el carrito
        public int CarritoId { get; set; }
        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;

        // Métodos helper
        public string GetClaveUnica()
        {
            if (string.IsNullOrEmpty(Color) && string.IsNullOrEmpty(Tamaño))
                return ProductoId.ToString();

            return $"{ProductoId}_{Color ?? ""}_{Tamaño ?? ""}";
        }

        public string ObtenerDescripcionVariante()
        {
            var partes = new List<string>();
            if (!string.IsNullOrEmpty(Color))
                partes.Add($"Color: {Color}");
            if (!string.IsNullOrEmpty(Tamaño))
                partes.Add($"Tamaño: {Tamaño}");
            return partes.Any() ? string.Join(" | ", partes) : "Sin variantes";
        }

        public string ObtenerResumen()
        {
            var variante = ObtenerDescripcionVariante();
            return $"{ProductoNombre} ({Cantidad} x ${PrecioUnitario:N2})" +
                   (variante != "Sin variantes" ? $" - {variante}" : "");
        }
    }
}