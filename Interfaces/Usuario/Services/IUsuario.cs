using Modelos.Response;

namespace Interfaces.Usuario.Services
{
    public interface IUsuario
    {
        Task<GeneralResponse> ConsultarUsuarios(int pagina, int registros, string correo);

        Task<GeneralResponse> RegistrarUsuario(string correo);

        Task<bool> ExisteUSuario(string correo);

        string GenerarToken(string correo);

        Task<bool> EsAdmin(string correo);

        string ObtenerCorreoToken(string token);
        Task<int> ObtenerId(string correo);
    }
}
