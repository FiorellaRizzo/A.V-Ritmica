namespace AVritmica.Shared.DTO
{
    public class CarritoProductoDTO
    {
        public int Id { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoImagen { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        // NUEVOS CAMPOS PARA VARIANTES
        public string? Color { get; set; }
        public string? Tamaño { get; set; }

        public string GetClaveUnica()
        {
            if (string.IsNullOrEmpty(Color) && string.IsNullOrEmpty(Tamaño))
                return $"{ProductoId}";
            return $"{ProductoId}_{Color ?? ""}_{Tamaño ?? ""}";
        }
    }
}