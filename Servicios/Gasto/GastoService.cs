using DBEF.Models;
using Interfaces.Gasto;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;
using Modelos.Response.Prestamo;

namespace Servicios.Gasto
{
    public class GastoService(AppDeudaContext db) : IGasto
    {
        private readonly AppDeudaContext _db = db;
        public async Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            var prestamos = await(from prestamo in _db.Prestamos
                                  where prestamo.IdUsuario == idUsuario &&
                                        ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null) 
                                  select new
                                  {
                                      Prestamo = prestamo,

                                  })
                                                  .OrderByDescending(p => p.Prestamo.FechaPrestamo)
                                                  .Skip((pagina) * registros)
                                                  .Take(registros)
                                                  .ToListAsync();

            var prestamoResponses = prestamos.Select(p => new PrestamoResponse
            {
                Descripcion = p.Prestamo.Descripcion.Trim(),
                FechaPrestamo = p.Prestamo.FechaPrestamo,
                Id = p.Prestamo.Id,
                Prestamo = p.Prestamo.MontoPrestamo,
            }).ToList();

            if (prestamoResponses.Count == 0)
            {
                return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoHayGastos);
            }

            return new GeneralResponse
            {
                Codigo = CodigoRespuesta.Exito,
                Data = prestamoResponses
            };
        }

        public async Task<GeneralResponse> Editar(PrestamoQuery gasto, int idUsuario)
        {
            try
            {
                var editar = await _db.Prestamos.Where(p => p.Id == gasto.Id && p.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoExiste);
                }

                editar.Descripcion = gasto.Descripcion.Trim();
                editar.MontoPrestamo = gasto.MontoPrestamo;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Actualizado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Eliminar(int idGasto, int idUsuario)
        {
            try
            {
                var eliminar = await _db.Prestamos.Where(p => p.Id == idGasto && p.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (eliminar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoExiste);
                }

                _db.Remove(eliminar);
                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Eliminado);
            }
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<bool> Existe(int idGasto, int idUsuario)
        {
            try
            {
                return await _db.Prestamos.Where(p => p.Id == idGasto && p.IdUsuario == idUsuario).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery gasto, int idUsuario)
        {
            try
            {

                await _db.Prestamos.AddAsync(new DBEF.Models.Prestamo()
                {
                    Descripcion = gasto.Descripcion.Trim(),
                    FechaPrestamo = Formatos.ObtenerFechaHoraLocal(),
                    IdUsuario = idUsuario,
                    IdDeudor = idUsuario,
                    PagoCompleto = true,
                    MontoPrestamo = gasto.MontoPrestamo,
                    Propio = true
                }
                     );
                await _db.SaveChangesAsync();
                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
