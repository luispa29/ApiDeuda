using Interfaces.Abono;
using Interfaces.Deudor.Service;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Microsoft.Extensions.Logging;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades;
using Utilidades.Helper;

namespace Logica.Prestamo
{
    public class PrestamoLogica(IPrestamo prestamo, IUsuario usuario, IDeudor deudor, IAbono abono, ILogger<PrestamoLogica> _logger) : IPrestamoLogica
    {
        private readonly IUsuario _usuario = usuario;
        private readonly IPrestamo _prestamo = prestamo;
        private readonly IDeudor _duedor = deudor;
        private readonly IAbono _abono = abono;

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

                var totlaPrestamos = await _prestamo.ConsularTotalPrestamo(idUsuario, IdDeudor, fechaDesdeConsulta, fechaHastaConsulta);


                var respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty);
                respuesta.Data = totlaPrestamos;
                respuesta.Contador = await _prestamo.PorCobrar(idUsuario);
                return respuesta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                consulta.Contador = await _prestamo.PorCobrar(idUsuario);
                return consulta;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> EditarPrestamo(PrestamoQuery prestamo, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existePrestamo = await _prestamo.ExistePrestamo(prestamo.Id, idUsuario);

                if (!existePrestamo) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExistePrestamo); }

                decimal totalAbonado = await _abono.TotalAbonoPrestamo(prestamo.Id, idUsuario);

                bool existeDeudor = await _duedor.ExisteDeudorId(prestamo.IdDeudor, idUsuario);
                bool pagoCompleto = prestamo.MontoPrestamo == totalAbonado;

                var valido = ValidarEdicionPrestamo(existeDeudor, token, prestamo.MontoPrestamo, totalAbonado);

                if (valido.Codigo == CodigoRespuesta.Exito)
                {
                    GeneralResponse editar = await _prestamo.Editar(prestamo, idUsuario, pagoCompleto);
                    editar.Token = token;

                    return editar;
                }
                else { return valido; }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                    GeneralResponse registrar = await _prestamo.Registrar(prestamo, idUsuario);
                    registrar.Token = token;

                    return registrar;
                }
                else
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExisteDeudor);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        private static GeneralResponse ValidarEdicionPrestamo(bool existeDeudor, string token, decimal montoPrestamo, decimal totalAbonado)
        {
            if (!existeDeudor) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExisteDeudor); }
            if (montoPrestamo < totalAbonado)
            {
                string mensaje = Formatos.ReemplazarTexto(MensajePrestamoHelper.ValorMenor, MensajeAbonoHelper.Abonado, totalAbonado.ToString());
                return Transaccion.Respuesta(CodigoRespuesta.MontoMayor, 0, token, mensaje);
            }

            return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty);
        }

        public async Task<GeneralResponse> Eliminar(int idPrestamo, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existePrestamo = await _prestamo.ExistePrestamo(idPrestamo, idUsuario);

                if (!existePrestamo) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExistePrestamo); }

                var eliminar = await _prestamo.Eliminar(idPrestamo, idUsuario);
                eliminar.Token = token;

                return eliminar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
