using Interfaces.Deudor.Service;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Modelos.Response.Prestamo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilidades;
using Utilidades.Helper;

namespace Logica.Prestamo
{
    public class PrestamoLogica(IPrestamo prestamo, IUsuario usuario, IDeudor deudor) : IPrestamoLogica
    {
        private readonly IUsuario _usuario = usuario;
        private readonly IPrestamo _prestamo = prestamo;
        private readonly IDeudor _duedor = deudor;

        public async Task<GeneralResponse> ConsularTotalPrestamo(string token, int? IdDeudor, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);
                token = _usuario.GenerarToken(correo);

                DateOnly? fechaDesdeConsulta = null;
                DateOnly? fechaHastaConsulta = null;

                if (fechaDesde != null && fechaHasta != null)
                {
                    fechaDesdeConsulta = Formatos.DevolverSoloFecha((DateTime)fechaDesde);
                    fechaHastaConsulta = Formatos.DevolverSoloFecha((DateTime)fechaHasta);
                }

                var totlaPrestamos = await _prestamo.ConsularTotalPrestamo(idUsuario,IdDeudor, fechaDesdeConsulta, fechaHastaConsulta);
              

                var respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty);
                respuesta.Data = totlaPrestamos;

                return respuesta;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, string token, DateTime? fechaDesde, DateTime? fechaHasta)
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

                GeneralResponse consulta = await _prestamo.ConsultarPrestamos(pagina, registros, IdDeudor, idUsuario, fechaDesdeConsulta, fechaHastaConsulta);

                consulta.Token = token;

                return consulta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);
                bool existeDeudor = await _duedor.ExisteDeudorId(prestamo.IdDeudor, idUsuario);

                token = _usuario.GenerarToken(correo);

                if (existeDeudor)
                {
                    GeneralResponse registrar = await _prestamo.RegistrarPrestamo(prestamo, idUsuario);
                    registrar.Token = token;

                    return registrar;
                }
                else
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExisteDeudor);
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
