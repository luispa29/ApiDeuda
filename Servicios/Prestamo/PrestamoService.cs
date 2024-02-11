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

        public async Task<GeneralResponse> ConsultarPrestamos(int pagina, int registros, int? IdDeudor, int idUsuario, DateOnly? fecha)
        {
            try
            {
                var prestamos = await (from prestamo in _db.Prestamos
                                       join deudor in _db.Deudores on prestamo.IdDeudor equals deudor.Id
                                       join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo into abonosGroup
                                       where prestamo.IdUsuario == idUsuario &&
                                             prestamo.PagoCompleto == false &&
                                             (prestamo.IdDeudor == IdDeudor || IdDeudor == null) &&
                                             ((prestamo.FechaPrestamo >= fecha && prestamo.FechaPrestamo <= fecha) || fecha == null)
                                       select new
                                       {
                                           Prestamo = prestamo,
                                           Deudor = deudor,
                                           Abonos = abonosGroup.DefaultIfEmpty() // Manejo para cuando no hay abonos asociados
                                       })
                                       .OrderByDescending(p => p.Prestamo.FechaPago)
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

        public async Task<GeneralResponse> RegistrarPrestamo(PrestamoQuery prestamo, int idUsuario)
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
                    FechaPago = prestamo.FechaPago != null ? Utilidades.Formatos.DevolverSoloFecha((global::System.DateTime)prestamo.FechaPago) : null,
                    FechaPrestamo = Utilidades.Formatos.ObtenerFechaHoraLocal(),
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
