using DBEF.Models;
using Interfaces.Abono;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modelos.Response;
using Modelos.Response.Abono;
using Utilidades;
using Utilidades.Helper;

namespace Servicios.Abono
{
    public class AbonoService(AppDeudaContext db, ILogger<AbonoService> _logger) : IAbono
    {
        private readonly AppDeudaContext _db = db;

        public async Task<GeneralResponse> Editar(decimal abono, int idAbono)
        {
            try
            {
                var editar = await _db.Abonos.FindAsync(idAbono);

                if (editar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeAbonoHelper.NoExiste);
                }

                editar.Abono1 = abono;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeAbonoHelper.Actualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Registrar(decimal abono, int idPrestamo)
        {
            try
            {
                await _db.Abonos.AddAsync(new DBEF.Models.Abono
                {
                    Abono1 = abono,
                    FechaAbono = Formatos.ObtenerFechaHoraLocal(),
                    IdPrestamo = idPrestamo,
                });

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeAbonoHelper.Registrado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<decimal> TotalAbonoPrestamo(int idPrestamo, int idUsuario)
        {
            try
            {
                return await (from prestamo in _db.Prestamos
                              join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo
                              where prestamo.IdUsuario == idUsuario && abono.IdPrestamo == idPrestamo
                              select new
                              {
                                  abono.Abono1
                              }).SumAsync(a => a.Abono1);
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public async Task<AbonoResponse?> AbonoEditar(int idAbono)
        {
            AbonoResponse abono = new();
            try
            {
                return await _db.Abonos.Where(a => a.Id == idAbono).Select(a => new AbonoResponse
                {
                    Abono = a.Abono1,
                    Id = a.Id,
                    IdPrestamo = a.IdPrestamo
                }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<GeneralResponse> Eliminar(int idAbono)
        {
            try
            {
                var eliminar = await _db.Abonos.FindAsync(idAbono);

                if (eliminar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeAbonoHelper.NoExiste);
                }

                _db.Remove(eliminar);
                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeAbonoHelper.Eliminado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> ConsultarAbonoPrestamo(int pagina, int registros, int idPrestamo)
        {
            try
            {
                var abonos = await _db.Abonos.Where(a => a.IdPrestamo == idPrestamo).Select(a => new AbonoResponse
                {
                    Abono = a.Abono1,
                    Id = a.Id,
                    Fecha = a.FechaAbono
                })
                .OrderByDescending(a => a.Fecha)
                .Skip((pagina) * registros)
                .Take(registros)
                .ToListAsync();

                if(abonos.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeAbonoHelper.NoExisteAbonos);
                }

                return new GeneralResponse
                {
                    Codigo = CodigoRespuesta.Exito,
                    Data = abonos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
