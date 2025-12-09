
namespace AVritmica.Shared.DTO
{
    public class CarritoResumenDTO
    {
        
        // TOTALES BÁSICOS
        
        public int TotalItems { get; set; }
        public int CantidadProductosUnicos { get; set; }
        public decimal Subtotal { get; set; }

        
        // INFORMACIÓN DEL CARRITO
        
        public int CarritoId { get; set; }
        public int UsuarioId { get; set; }
        public string Estado { get; set; } = "Activo";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        
        // PROPIEDADES CALCULADAS
        
        public bool TieneItems => TotalItems > 0;

        // Para compatibilidad (alias de Subtotal)
        public decimal Total => Subtotal;
        public decimal TotalPrecio => Subtotal;

       
        // MÉTODOS
    

       
        /// Calcula los totales basados en una lista de items
       
        public void CalcularDesdeItems(List<CarritoItemDTO> items)
        {
            if (items == null || !items.Any())
            {
                TotalItems = 0;
                CantidadProductosUnicos = 0;
                Subtotal = 0;
                return;
            }

            TotalItems = items.Sum(i => i.Cantidad);
            CantidadProductosUnicos = items.Count;
            Subtotal = items.Sum(i => i.Subtotal);
        }

       
        /// Obtiene un resumen en texto para mostrar
        
        public string ObtenerResumenTexto()
        {
            if (!TieneItems)
                return "Carrito vacío";

            return $"{CantidadProductosUnicos} producto(s) - {TotalItems} item(s) - Total: ${Subtotal:N2}";
        }

        
        /// Obtiene un resumen corto (para badges/iconos)
        
        public string ObtenerResumenCorto()
        {
            if (!TieneItems)
                return "0";

            return TotalItems > 99 ? "99+" : TotalItems.ToString();
        }
    }
}