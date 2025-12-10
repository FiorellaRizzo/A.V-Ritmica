using System.Threading.Tasks;

namespace AVritmica.Client.Servicios
{
    public interface ISesionUsuarioServicio
    {
        Task<int?> ObtenerUsuarioIdAsync();
        Task GuardarUsuarioIdAsync(int usuarioId);
        Task BorrarUsuarioIdAsync();
    }
}
