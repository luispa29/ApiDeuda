using DBEF.Models;
using Interfaces.Deudor.Service;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;

namespace Servicios.Deudor
{
    public class DeudorService(AppDeudaContext db) : IDeudor
    {
        private readonly AppDeudaContext _db = db;

        public async Task<GeneralResponse> CambiarEstadoDeudor(int idDeudor, int idUsuario, bool existe)
        {
            try
            {
                var respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajesDeudorHelper.Eliminado);

                var editar = await _db.Deudores.Where(d => d.Id == idDeudor && d.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar != null)
                {

                    editar.Estado = existe == true || !editar.Estado;

                    await _db.SaveChangesAsync();
                }

                return respuesta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EditarDeudor(string deudor, int idDeudor, int idUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ExisteDeudor(string deudor, int idUsuario)
        {
            try
            {
                int existe = await _db.Deudores.Where(d => d.Nombres.ToUpper().Trim() == deudor.ToUpper().Trim() && d.IdUsuario == idUsuario).Select(d=> d.Id).FirstOrDefaultAsync();
                return existe;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<GeneralResponse> RegistrarDeudor(string deudor, int idUsuario)
        {
            try
            {
                var nuevoDeudor = new Deudore
                {
                    Estado = true,
                    IdUsuario = idUsuario,
                    Nombres = deudor.Trim()
                };

                await _db.Deudores.AddAsync(nuevoDeudor);
                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajesDeudorHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
