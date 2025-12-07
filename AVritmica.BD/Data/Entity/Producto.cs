using AVritmica.BD.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVritmica.BD.Data.Entity
{
    [Index(nameof(Nombre), Name = "IX_Productos_Nombre")]
    public class Producto : EntityBase
    {
        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string ImagenUrl { get; set; } = string.Empty;

        // Clave foránea
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public List<CarritoProducto> CarritoProductos { get; set; } = new List<CarritoProducto>();
        public List<StockMovimiento> StockMovimientos { get; set; } = new List<StockMovimiento>();
        public List<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();

        // NUEVAS PROPIEDADES PARA VARIANTES 
        [MaxLength(500)]
        public string ColoresDisponibles { get; set; } = string.Empty;

        [MaxLength(500)]
        public string TamaniosDisponibles { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string ImagenesVariantes { get; set; } = string.Empty;

        public bool TieneVariantes { get; set; } = false;

        // NUEVA PROPIEDAD PARA STOCK POR COLOR
        [MaxLength(2000)]
        public string StockPorColor { get; set; } = string.Empty;

        // Método helper para obtener lista de colores
        public List<string> ObtenerColoresLista()
        {
            if (string.IsNullOrWhiteSpace(ColoresDisponibles))
                return new List<string>();

            return ColoresDisponibles
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();
        }

        // Método helper para obtener lista de tamaños
        public List<string> ObtenerTamaniosLista()
        {
            if (string.IsNullOrWhiteSpace(TamaniosDisponibles))
                return new List<string>();

            return TamaniosDisponibles
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();
        }

        // Método helper para obtener lista de imágenes variantes
        public List<string> ObtenerImagenesVariantesLista()
        {
            if (string.IsNullOrWhiteSpace(ImagenesVariantes))
                return new List<string>();

            return ImagenesVariantes
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .ToList();
        }

        //  MÉTODOS NUEVOS PARA STOCK POR COLOR 

        public Dictionary<string, int> ObtenerStockPorColorDiccionario()
        {
            var diccionario = new Dictionary<string, int>();

            if (string.IsNullOrWhiteSpace(StockPorColor))
                return diccionario;

            var items = StockPorColor.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                var partes = item.Split(':');
                if (partes.Length == 2)
                {
                    var color = partes[0].Trim();
                    if (int.TryParse(partes[1].Trim(), out int stock))
                    {
                        diccionario[color] = stock;
                    }
                }
            }

            return diccionario;
        }

        public int ObtenerStockPorColor(string color)
        {
            var diccionario = ObtenerStockPorColorDiccionario();
            return diccionario.ContainsKey(color) ? diccionario[color] : 0;
        }

        public void ActualizarStockPorColor(string color, int nuevoStock)
        {
            var diccionario = ObtenerStockPorColorDiccionario();
            diccionario[color] = nuevoStock;

            // Reconstruir el string
            StockPorColor = string.Join(";",
                diccionario.Select(kv => $"{kv.Key}:{kv.Value}"));

            // Actualizar el stock total (suma de todos los colores)
            Stock = diccionario.Values.Sum();
        }

        public void InicializarStockPorColor()
        {
            if (!TieneVariantes || string.IsNullOrWhiteSpace(ColoresDisponibles))
                return;

            var diccionario = ObtenerStockPorColorDiccionario();
            var colores = ObtenerColoresLista();

            // Si no hay stock configurado, distribuirlo equitativamente
            if (diccionario.Count == 0 && Stock > 0)
            {
                foreach (var color in colores)
                {
                    int stockPorColor = Stock / Math.Max(colores.Count, 1);
                    diccionario[color] = stockPorColor;
                }

                StockPorColor = string.Join(";",
                    diccionario.Select(kv => $"{kv.Key}:{kv.Value}"));
            }
        }

        //  PROPIEDADES CALCULADAS

        [NotMapped] //NO se guarda en la base de datos
        public bool AlgunColorAgotado
        {
            get
            {
                if (!TieneVariantes) return false;
                if (string.IsNullOrWhiteSpace(ColoresDisponibles)) return false;

                var diccionario = ObtenerStockPorColorDiccionario();
                if (diccionario.Count == 0) return Stock <= 0;

                return diccionario.Any(kv => kv.Value <= 0);
            }
        }

        [NotMapped]
        public bool CompletamenteAgotado
        {
            get
            {
                if (!TieneVariantes) return Stock <= 0;

                var diccionario = ObtenerStockPorColorDiccionario();
                if (diccionario.Count == 0) return Stock <= 0;

                return diccionario.All(kv => kv.Value <= 0) || Stock <= 0;
            }
        }

        [NotMapped]
        public List<string> ColoresConStockLista
        {
            get
            {
                if (!TieneVariantes) return new List<string>();

                var diccionario = ObtenerStockPorColorDiccionario();
                return diccionario.Where(kv => kv.Value > 0)
                                 .Select(kv => kv.Key)
                                 .ToList();
            }
        }

        [NotMapped]
        public List<string> ColoresAgotadosLista
        {
            get
            {
                if (!TieneVariantes) return new List<string>();

                var diccionario = ObtenerStockPorColorDiccionario();
                return diccionario.Where(kv => kv.Value <= 0)
                                 .Select(kv => kv.Key)
                                 .ToList();
            }
        }

        // Método para consumir stock de un color específico
        public bool ConsumirStockColor(string color, int cantidad)
        {
            if (!TieneVariantes)
            {
                // Si no tiene variantes, usar el stock general
                if (Stock >= cantidad)
                {
                    Stock -= cantidad;
                    return true;
                }
                return false;
            }

            var stockActual = ObtenerStockPorColor(color);
            if (stockActual >= cantidad)
            {
                ActualizarStockPorColor(color, stockActual - cantidad);
                return true;
            }
            return false;
        }

        // Método para reponer stock de un color específico
        public void ReponerStockColor(string color, int cantidad)
        {
            if (!TieneVariantes)
            {
                Stock += cantidad;
                return;
            }

            var stockActual = ObtenerStockPorColor(color);
            ActualizarStockPorColor(color, stockActual + cantidad);
        }

        // Método para verificar si un color específico está disponible
        public bool ColorDisponible(string color, int cantidadRequerida = 1)
        {
            if (!TieneVariantes) return Stock >= cantidadRequerida;

            return ObtenerStockPorColor(color) >= cantidadRequerida;
        }
    }
}