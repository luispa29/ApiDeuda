using Interfaces.Presupuesto;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Interfaces.Usuario.Services;
using Microsoft.Extensions.Logging;

namespace Logica.Presupueso
{
    public class PresupuestoLogica (IPresupuesto presupuesto, IUsuario usuario, ILogger<PresupuestoLogica> _logger) : IPresupuestoLogica
    {
        private readonly IPresupuesto _presupuesto = presupuesto;
        private readonly IUsuario _usuario = usuario;

        public async Task<GeneralResponse> Actualizar(string token, decimal presupuesto)
        {
            try
            {
                GeneralResponse actualizar = new();

                DateOnly fecha = Formatos.ObtenerFechaHoraLocal();

                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                decimal presupuestoInicial = await _presupuesto.Obtener(idUsuario, fecha.Month, fecha.Year);

                if(presupuestoInicial == 0) return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePresupuestoHelper.NoExiste);

                actualizar =  await _presupuesto.Actualizar(idUsuario, fecha.Month, fecha.Year, presupuesto);
                actualizar.Token = _usuario.GenerarToken(correo);

                return actualizar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Registrar(string token, decimal presupuesto)
        {
            try
            {
                GeneralResponse registrar = new();

                DateOnly fecha = Formatos.ObtenerFechaHoraLocal();

                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                decimal presupuestoInicial = await _presupuesto.Obtener(idUsuario, fecha.Month,fecha.Year);
                
                decimal presupuestoFinal = presupuesto + presupuestoInicial;

                registrar = presupuestoInicial == 0 ? await _presupuesto.Registrar(idUsuario, fecha.Month, fecha.Year,presupuesto) : await _presupuesto.Actualizar(idUsuario, fecha.Month, fecha.Year, presupuestoFinal);

                registrar.Token = _usuario.GenerarToken(correo);

                registrar.Mensaje = registrar.Codigo == CodigoRespuesta.Exito ? MensajePresupuestoHelper.Registrado : registrar.Mensaje;
                
                return registrar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
