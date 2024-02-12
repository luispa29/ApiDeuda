using DBEF.Models;
using Interfaces.Prestamo;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Modelos.Response.Prestamo;
using Microsoft.EntityFrameworkCore;

namespace Servicios.Prestamo
{
    public class PrestamoService(AppDeudaContext db) : IPrestamo
    {
        private readonly AppDeudaContext _db = db;

        public async Task<ValoresPrestamosResponse> ConsularTotalPrestamo(int idUsuario, int? IdDeudor, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            ValoresPrestamosResponse totales = new();
            try
            {

                var consulta = await (from prestamo in _db.Prestamos
                                      join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo into abonoGroup
                                      where prestamo.IdUsuario == idUsuario &&
                                      (prestamo.IdDeudor == IdDeudor || IdDeudor == null) &&
                                      ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null)
                                      select new
                                      {
                                          Prestamos = prestamo,
                                          Abonos = abonoGroup.DefaultIfEmpty()
                                      }).ToListAsync();

                if (consulta.Count == 0)
                {
                    return totales;
                }

                totales.Prestado = consulta.Select(c => new { Valor = c.Prestamos.MontoPrestamo }).Sum(c => c.Valor);
                totales.Cobrado = consulta.Select(c => new { Valor = c.Abonos.Sum(a => a?.Abono1 ?? 0) }).Sum(c => c.Valor);

                return totales;
            }
            catch (Exception)
            {

                return totales;
            }
        }

        public async Task<decimal> ConsultarMontoPrestamo(int idPrestamo)
        {
            try
            {
                return await _db.Prestamos.Where(p => p.Id == idPrestamo).Select(p => p.MontoPrestamo).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public async Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            try
            {
                var prestamos = await (from prestamo in _db.Prestamos
                                       join deudor in _db.Deudores on prestamo.IdDeudor equals deudor.Id
                                       join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo into abonosGroup
                                       where prestamo.IdUsuario == idUsuario &&
                                             prestamo.PagoCompleto == false &&
                                             (prestamo.IdDeudor == IdDeudor || IdDeudor == null) &&
                                             ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null)
                                       select new
                                       {
                                           Prestamo = prestamo,
                                           Deudor = deudor,
                                           Abonos = abonosGroup.DefaultIfEmpty() // Manejo para cuando no hay abonos asociados
                                       })
                                       .OrderByDescending(p => p.Prestamo.FechaPago).ThenBy(p => p.Deudor.Nombres)
                                       .Skip((pagina) * registros)
                                       .Take(registros)
                                       .ToListAsync();

                var prestamoResponses = prestamos.Select(p => new PrestamoResponse
                {
                    Descripcion = p.Prestamo.Descripcion.Trim(),
                    Completo = p.Prestamo.PagoCompleto,
                    Deudor = p.Deudor.Nombres.ToUpper().Trim(),
                    FechaPago = p.Prestamo.FechaPago,
                    FechaPrestamo = p.Prestamo.FechaPrestamo,
                    Id = p.Prestamo.Id,
                    Imgen = p.Prestamo.ImagenUrl,
                    ImgenId = p.Prestamo.ImagenId,
                    Prestamo = p.Prestamo.MontoPrestamo,
                    Abono = p.Abonos.Sum(a => a?.Abono1 ?? 0) // Suma de los abonos o 0 si es nulo
                }).ToList();

                if (prestamoResponses.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.NoHayPrestamos);
                }

                return new GeneralResponse
                {
                    Codigo = CodigoRespuesta.Exito,
                    Data = prestamoResponses
                };
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Editar(PrestamoQuery prestamo, int idUsuario, bool pagoCompleto)
        {
            try
            {
                var editar = await _db.Prestamos.Where(p => p.Id == prestamo.Id && p.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (editar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.NoExistePrestamo);
                }

                editar.IdDeudor = prestamo.IdDeudor;
                editar.Descripcion = prestamo.Descripcion.Trim();
                editar.ImagenUrl = prestamo.ImagenUrl;
                editar.ImagenId = prestamo.ImagenId;
                editar.FechaPago = prestamo.FechaPago != null ? Formatos.DevolverSoloFecha((global::System.DateTime)prestamo.FechaPago) : null;
                editar.MontoPrestamo =prestamo.MontoPrestamo;
                editar.PagoCompleto = pagoCompleto;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Registrado);
            }
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery prestamo, int idUsuario)
        {
            try
            {
                await _db.Prestamos.AddAsync(new DBEF.Models.Prestamo
                {
                    Descripcion = prestamo.Descripcion.Trim(),
                    IdDeudor = prestamo.IdDeudor,
                    ImagenUrl = prestamo.ImagenUrl,
                    ImagenId = prestamo.ImagenId,
                    MontoPrestamo = prestamo.MontoPrestamo,
                    FechaPago = prestamo.FechaPago != null ? Formatos.DevolverSoloFecha((global::System.DateTime)prestamo.FechaPago) : null,
                    FechaPrestamo = Formatos.ObtenerFechaHoraLocal(),
                    IdUsuario = idUsuario,
                    PagoCompleto = false
                });

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
