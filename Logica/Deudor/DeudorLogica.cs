using Interfaces.Deudor.Logica;
using Interfaces.Deudor.Service;
using Interfaces.Usuario.Services;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;


namespace Logica.Deudor
{
    public class DeudorLogica (IDeudor deudor, IUsuario usuario ): IDeudorLogica
    {
        private readonly IDeudor _deudor = deudor;
        private readonly IUsuario _usuario = usuario;

        public Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, string token)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EditarDeudor(CatalogoResponse deudor, string token)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EliminarDeudor(int idDeudor, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> RegistrarDeudor(string deudor, string token)
        {
            try
            {
                var correo = _usuario.ObtenerCorreoToken(token);
                var idUsuario = await _usuario.ObtenerId(correo);
                var idDeudor = await _deudor.ExisteDeudor(deudor, idUsuario);
                
                token = _usuario.GenerarToken(correo);
                
                if (idDeudor > 0)
                {
                    var cambiarEstado = await _deudor.CambiarEstadoDeudor(idDeudor,idUsuario,true);
                    return Transaccion.Respuesta(CodigoRespuesta.Error, 0, token, MensajesDeudorHelper.Registrado);
                }
                else
                {
                    var registrar = await _deudor.RegistrarDeudor(deudor, idUsuario);
                    registrar.Token = token;

                    return registrar;
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
