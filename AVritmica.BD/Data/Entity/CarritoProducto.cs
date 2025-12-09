using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVritmica.BD.Data.Entity
{
    public class CarritoProducto : EntityBase
    {
        // Claves foráneas (¡IMPORTANTE!)
        [Required]
        public int CarritoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        // Propiedades
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [MaxLength(50)]
        public string? Tamaño { get; set; }

        // Propiedades de navegación (¡IMPORTANTE!)
        public Carrito? Carrito { get; set; }
        public Producto? Producto { get; set; }
    }
}