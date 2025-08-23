using Interfaces.Usuario;
using Interfaces.Usuario.Services;
using Microsoft.Extensions.Logging;
using Modelos.Response;
using Utilidades;
using Utilidades.Helper;

namespace Logica.Usuario
{
    public class UsuarioLogica(IUsuario usuario, ILogger<UsuarioLogica> _logger) : IUsuarioLogica
    {
        private readonly IUsuario _usuario = usuario;
        public async Task<GeneralResponse> ConsultarUsuarios(int pagina, int registros, string token)
        {
            string correo = _usuario.ObtenerCorreoToken(token);

            bool admin = await _usuario.EsAdmin(correo);

            if (admin)
            {
                return await _usuario.ConsultarUsuarios(pagina, registros, correo);
            }
            else
            {
                token = _usuario.GenerarToken(correo);

                return Transaccion.Respuesta(CodigoRespuesta.BloqueoRol, 0, token, MensajesUsuariosHelper.OpcionNoDisponible);
            }
        }

        public async Task<GeneralResponse> Login(string correo)
        {
            try
            {
                var token = _usuario.GenerarToken(correo);
                var existe = await _usuario.Login(correo);
                if (existe != null)
                {
                    await _usuario.RegistrarCodigoCompartido(correo);

                    return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty,existe.Valor);
                }
                else
                {
                    var registrar = _usuario.RegistrarUsuario(correo);

                    return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
