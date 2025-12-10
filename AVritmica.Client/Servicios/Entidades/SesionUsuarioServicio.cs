using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace AVritmica.Client.Servicios
{
    public class SesionUsuarioServicio : ISesionUsuarioServicio
    {
        private readonly IJSRuntime js;
        private const string ClaveUsuario = "usuarioActualId";

        public SesionUsuarioServicio(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task<int?> ObtenerUsuarioIdAsync()
        {
            var valor = await js.InvokeAsync<string>("localStorage.getItem", ClaveUsuario);

            if (string.IsNullOrEmpty(valor))
                return null;

            if (int.TryParse(valor, out int id))
                return id;

            return null;
        }

        public async Task GuardarUsuarioIdAsync(int usuarioId)
        {
            await js.InvokeVoidAsync("localStorage.setItem", ClaveUsuario, usuarioId.ToString());
        }

        public async Task BorrarUsuarioIdAsync()
        {
            await js.InvokeVoidAsync("localStorage.removeItem", ClaveUsuario);
        }
    }
}
