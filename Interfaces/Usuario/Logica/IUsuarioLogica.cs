using Modelos.Response;

namespace Interfaces.Usuario
{
    public interface IUsuarioLogica
    {
        Task<GeneralResponse> ConsultarUsuarios(int pagina, int registros, string token);

        Task<GeneralResponse> Login(string correo);
    }
}
