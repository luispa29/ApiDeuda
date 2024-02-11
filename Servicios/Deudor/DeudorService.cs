using DBEF.Models;
using Interfaces.Deudor.Service;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;
using Modelos.Response.Usuario;

namespace Servicios.Deudor
{
    public class DeudorService(AppDeudaContext db) : IDeudor
    {
        private readonly AppDeudaContext _db = db;

        public async Task<GeneralResponse> CambiarEstadoDeudor(int idDeudor, int idUsuario, bool estado)
        {
            try
            {
                var respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajesDeudorHelper.Eliminado);

                var editar = await _db.Deudores.Where(d => d.Id == idDeudor && d.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar != null)
                {
                    editar.Estado = estado;

                    await _db.SaveChangesAsync();
                }

                return respuesta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, int idUsuario)
        {
            GeneralResponse respuesta = new();
            try
            {
                var usuarios = await _db.Deudores.Where(d=> d.IdUsuario == idUsuario).Select(d => new CatalogoResponse { Codigo = d.Id, Valor = d.Nombres.Trim() })
                                                   .Skip((pagina) * registros)
                                                   .Take(registros)
                                                   .ToListAsync();

                if (usuarios.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajesDeudorHelper.NoHayDeudoresRegistrados);
                }

                respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, string.Empty);
                respuesta.Data = usuarios;

                return respuesta;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> EditarDeudor(string deudor, int idDeudor, int idUsuario)
        {
            try
            {
                var respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajesDeudorHelper.Actualizado);

                var editar = await _db.Deudores.Where(d => d.Id == idDeudor && d.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar != null)
                {
                    editar.Nombres = deudor.Trim();

                    await _db.SaveChangesAsync();
                }

                return respuesta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
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
