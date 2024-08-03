
using Interfaces.Deudor.Service;
using Interfaces.Gasto;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Query.Prestamo;
using Modelos.Response;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades.Helper;
using Utilidades;

namespace Logica.Gasto
{
    public class GastoLogica(IGasto gasto, IUsuario usuario) : IGastoLogica
    {
        private readonly IUsuario _usuario = usuario;
        private readonly IGasto _gasto = gasto;

        public async Task<GeneralResponse> Consultar(int pagina, int registros, string token, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                token = _usuario.GenerarToken(correo);

                int idUsuario = await _usuario.ObtenerId(correo);

                DateOnly? fechaDesdeConsulta = null;
                DateOnly? fechaHastaConsulta = null;

                if (fechaDesde != null && fechaHasta != null)
                {
                    fechaDesdeConsulta = Formatos.DevolverSoloFecha((DateTime)fechaDesde);
                    fechaHastaConsulta = Formatos.DevolverSoloFecha((DateTime)fechaHasta);
                }

                GeneralResponse consulta = await _gasto.ConsultarGastos(pagina, registros, idUsuario, fechaDesdeConsulta, fechaHastaConsulta);

                consulta.Token = token;
                return consulta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Editar(PrestamoQuery gasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existe = await _gasto.Existe(gasto.Id, idUsuario);

                if (!existe) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajeGastoHelper.NoExiste); }

                    GeneralResponse editar = await _gasto.Editar(gasto, idUsuario);
                    editar.Token = token;

                    return editar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Eliminar(int idGasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existe = await _gasto.Existe(idGasto, idUsuario);

                if (!existe) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajeGastoHelper.NoExiste); }

                GeneralResponse eliminar = await _gasto.Eliminar(idGasto, idUsuario);
                eliminar.Token = token;

                return eliminar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery gasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                    GeneralResponse registrar = await _gasto.Registrar(gasto, idUsuario);
                    registrar.Token = token;
                    return registrar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
