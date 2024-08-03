
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

        public Task<GeneralResponse> Consultar(int pagina, int registros, string token, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> Editar(PrestamoQuery gasto, string token)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> Eliminar(int idPrestamo, string token)
        {
            throw new NotImplementedException();
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
