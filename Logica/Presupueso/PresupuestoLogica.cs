using Interfaces.Presupuesto;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Interfaces.Usuario.Services;

namespace Logica.Presupueso
{
    public class PresupuestoLogica (IPresupuesto presupuesto, IUsuario usuario) : IPresupuestoLogica
    {
        private readonly IPresupuesto _presupuesto = presupuesto;
        private readonly IUsuario _usuario = usuario;

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
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
