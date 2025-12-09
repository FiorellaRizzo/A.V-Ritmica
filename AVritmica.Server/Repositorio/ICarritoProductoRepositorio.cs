using AVritmica.BD.Data.Entity;

namespace AVritmica.Server.Repositorio
{
    public interface ICarritoProductoRepositorio
    {
        // ========== MÉTODOS EXISTENTES ==========
        Task<List<CarritoProducto>> Select();
        Task<CarritoProducto?> SelectById(int id);
        Task<List<CarritoProducto>> SelectByCarrito(int carritoId);
        Task<List<CarritoProducto>> SelectByProducto(int productoId);

        // ========== MÉTODOS ACTUALIZADOS PARA VARIANTES ==========

        // Método ORIGINAL (mantener para compatibilidad)
        Task<CarritoProducto?> SelectByCarritoAndProducto(int carritoId, int productoId);

        // NUEVO MÉTODO con soporte para variantes
        Task<CarritoProducto?> SelectByCarritoAndProductoConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null);

        Task<bool> Existe(int id);

        // Método ORIGINAL (mantener para compatibilidad)
        Task<bool> Existe(int carritoId, int productoId);

        // NUEVO MÉTODO con soporte para variantes
        Task<bool> ExisteConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null);

        Task<int> Insert(CarritoProducto entidad);
        Task<bool> Update(int id, CarritoProducto entidad);
        Task<bool> Delete(int id);

        // Método ORIGINAL (mantener para compatibilidad)
        Task<bool> DeleteByCarritoAndProducto(int carritoId, int productoId);

        // NUEVO MÉTODO con soporte para variantes
        Task<bool> DeleteByCarritoAndProductoConVariantes(
            int carritoId,
            int productoId,
            string? color = null,
            string? tamaño = null);

        Task<bool> ActualizarCantidad(int id, int cantidad);

        // Actualizar cantidad considerando variantes
        Task<bool> ActualizarCantidadConVariantes(
            int carritoId,
            int productoId,
            int cantidad,
            string? color = null,
            string? tamaño = null);

        Task<bool> ActualizarPrecioUnitario(int id, decimal precioUnitario);
        Task<int> ObtenerCantidadTotalEnCarritos(int productoId);

        //  MÉTODOS PARA EL CARRITO

        
        /// Vacía todos los productos de un carrito
      
        Task<bool> VaciarCarrito(int carritoId);

        
        /// Obtiene la cantidad total de items en un carrito
        
        Task<int> ObtenerCantidadTotalPorCarrito(int carritoId);

        
        /// Obtiene el monto total de un carrito
        
        Task<decimal> ObtenerTotalPorCarrito(int carritoId);

        
        /// Agrega o actualiza un producto en el carrito considerando variantes
        
        Task<bool> AgregarOActualizarProducto(
            int carritoId,
            int productoId,
            int cantidad,
            decimal precioUnitario,
            string? color = null,
            string? tamaño = null);
    }
}