using Interfaces.Deudor.Logica;
using Interfaces.Deudor.Service;
using Interfaces.Usuario.Services;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Interfaces.Prestamo;

namespace Logica.Deudor
{
    public class DeudorLogica(IDeudor deudor, IUsuario usuario, IPrestamo prestamo) : IDeudorLogica
    {
        private readonly IDeudor _deudor = deudor;
        private readonly IUsuario _usuario = usuario;
        private readonly IPrestamo _prestamo = prestamo;

        public async Task<GeneralResponse> ConsultarDeudores(string token)
        {
            try
            {
                var correo = _usuario.ObtenerCorreoToken(token);
                var idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);
                
                var consulta = await _deudor.ConsultarDeudores(idUsuario);
                consulta.Token = token;
                consulta.Contador = await _prestamo.PorCobrar(idUsuario);
                return consulta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> EditarDeudor(CatalogoResponse deudor, string token)
        {
            try
            {
                var correo = _usuario.ObtenerCorreoToken(token);
                var idUsuario = await _usuario.ObtenerId(correo);
                var idDeudor = await _deudor.ExisteDeudor(deudor.Valor, idUsuario);

                token = _usuario.GenerarToken(correo);

                if (idDeudor > 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, MensajesDeudorHelper.Actualizado);
                }
                else
                {
                    var actualizar = await _deudor.EditarDeudor(deudor.Valor, deudor.Codigo, idUsuario);
                    actualizar.Token = token;
                    return actualizar;
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> EliminarDeudor(int idDeudor, string token)
        {
            try
            {
                var correo = _usuario.ObtenerCorreoToken(token);
                var idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);
               
                var cambiarEstado = await _deudor.CambiarEstadoDeudor(idDeudor, idUsuario, false);
                
                return cambiarEstado;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
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
                    var cambiarEstado = await _deudor.CambiarEstadoDeudor(idDeudor, idUsuario, true);
                    return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, MensajesDeudorHelper.Registrado);
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
