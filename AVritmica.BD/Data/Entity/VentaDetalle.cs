using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.BD.Data.Entity
{
    public class VentaDetalle
    {
        [Key]
        public int Id { get; set; }

        // RELACIÓN CON VENTA
        [Required]
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        // RELACIÓN CON PRODUCTO
        [Required]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        // VARIANTES DEL PRODUCTO
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Tamanio { get; set; } = string.Empty;

        // CANTIDAD Y PRECIO
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // MÉTODO PARA CALCULAR SUBTOTAL
        public void CalcularSubtotal()
        {
            Subtotal = Cantidad * PrecioUnitario;
        }
    }
}
