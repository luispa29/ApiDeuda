using Interfaces.Deudor.Service;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Query.Prestamo;
using Modelos.Response;
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
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token,MensajePrestamoHelper.NoExisteDeudor);
                }
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
