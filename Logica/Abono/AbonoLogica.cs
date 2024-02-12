﻿using Interfaces.Abono;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Modelos.Response.Abono;

namespace Logica.Abono
{
    public class AbonoLogica(IAbono abono, IPrestamo prestamo, IUsuario usuario) : IAbonoLogica
    {
        private readonly IAbono _abono = abono;
        private readonly IPrestamo _prestamo = prestamo;
        private readonly IUsuario _usuario = usuario;

        public async Task<GeneralResponse> Registrar(decimal abono, int idPrestamo, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);
                bool existePrestamo = await _prestamo.ExistePrestamo(idPrestamo, idUsuario);
                decimal valorPrestamo = await _prestamo.ConsultarMontoPrestamo(idPrestamo);
                decimal totalAbonado = await _abono.TotalAbonoPrestamo(idPrestamo, idUsuario);

                token = _usuario.GenerarToken(correo);

                var valido = ValidarAbono(existePrestamo, abono, totalAbonado, valorPrestamo, token);

                if (valido.Codigo == CodigoRespuesta.Exito)
                {
                    var registrar = await _abono.Registrar(abono, idPrestamo);
                    registrar.Token = token;

                    return registrar;
                }
                else
                {
                    return valido;
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Editar(decimal abono, int idAbono, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);
                AbonoResponse abonoEditar = await _abono.AbonoEditar(idAbono);
                if (abonoEditar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeAbonoHelper.NoExiste);
                }
                bool existePrestamo = await _prestamo.ExistePrestamo(abonoEditar.IdPrestamo, idUsuario);
                decimal valorPrestamo = await _prestamo.ConsultarMontoPrestamo(abonoEditar.IdPrestamo);
                decimal totalAbonado = await _abono.TotalAbonoPrestamo(abonoEditar.IdPrestamo, idUsuario);

                totalAbonado -= abonoEditar.Abono;

                token = _usuario.GenerarToken(correo);

                var valido = ValidarAbono(existePrestamo, abono, totalAbonado, valorPrestamo, token);

                if (valido.Codigo == CodigoRespuesta.Exito)
                {
                    var editar = await _abono.Editar(abono, idAbono);
                    editar.Token = token;

                    return editar;
                }
                else
                {
                    return valido;
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        private static GeneralResponse ValidarAbono(bool existePrestamo, decimal montoAbonar, decimal totalAbonado, decimal valorPrestamo, string token)
        {
            try
            {
                if (!existePrestamo)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajePrestamoHelper.NoExistePrestamo);
                }

                if ((montoAbonar + totalAbonado) > valorPrestamo)
                {
                    string mensaje = MensajeAbonoHelper.AbonoSuperaPrestamo;
                    mensaje = Formatos.ReemplazarTexto(mensaje, MensajeAbonoHelper.Abono, montoAbonar.ToString());
                    mensaje = Formatos.ReemplazarTexto(mensaje, MensajeAbonoHelper.Abonado, totalAbonado.ToString());
                    mensaje = Formatos.ReemplazarTexto(mensaje, MensajeAbonoHelper.Prestamo, valorPrestamo.ToString());

                    return Transaccion.Respuesta(CodigoRespuesta.MontoMayor, 0, token, mensaje);
                }

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, string.Empty);

            }
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

    }
}
