﻿using DBEF.Models;
using Interfaces.Prestamo;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Modelos.Response.Prestamo;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Forms;

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
                var ignorados = await ObtenerIgnorados(idUsuario);

                var ignorar = ObtenerCodigosIgnorados(ignorados);

                var consulta = await (from prestamo in _db.Prestamos
                                      join abono in _db.Abonos on prestamo.Id equals abono.IdPrestamo into abonoGroup
                                      where prestamo.IdUsuario == idUsuario &&
                                      (prestamo.IdDeudor == IdDeudor || IdDeudor == null) &&
                                      ((prestamo.FechaPago >= fechaDesde && prestamo.FechaPago <= fechaHasta) || fechaDesde == null || fechaHasta == null) &&
                                      !ignorar.Contains(prestamo.IdDeudor) && prestamo.Propio == false
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
                                             ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null) && prestamo.Propio == false
                                       select new
                                       {
                                           Prestamo = prestamo,
                                           Deudor = deudor,
                                           Abonos = abonosGroup.DefaultIfEmpty() // Manejo para cuando no hay abonos asociados
                                       })
                                       .OrderBy(p => p.Prestamo.FechaPago).ThenBy(p => p.Deudor.Nombres)
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
                    IdDeudor = p.Deudor.Id,
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
                editar.MontoPrestamo = prestamo.MontoPrestamo;
                editar.PagoCompleto = pagoCompleto;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Actualizado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Eliminar(int idPrestamo, int idUsuario)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var eliminar = await _db.Prestamos.Where(p => p.Id == idPrestamo && p.IdUsuario == idUsuario).FirstOrDefaultAsync();

                if (eliminar == null)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.NoExistePrestamo);
                }

                var abonos = await _db.Abonos.Where(a => a.IdPrestamo == idPrestamo).ToListAsync();

                if (abonos.Count > 0)
                {
                    _db.RemoveRange(abonos);
                    await _db.SaveChangesAsync();
                }

                _db.Remove(eliminar);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Eliminado);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<bool> ExistePrestamo(int idPrestamo, int idUsuario)
        {
            try
            {
                return await _db.Prestamos.Where(p => p.Id == idPrestamo && p.IdUsuario == idUsuario).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> MarcarComoPagado(int idPrestamo, int idUsuario)
        {
            try
            {
                var prestamo = await _db.Prestamos.Where(p => p.Id == idPrestamo && p.IdUsuario == idUsuario).FirstOrDefaultAsync();
                if (prestamo != null)
                {
                    prestamo.PagoCompleto = true;

                    await _db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<int> PorCobrar(int idUsuario)
        {
            int porCobrar = 0;

            try
            {
                var ignorados = await ObtenerIgnorados(idUsuario);

                var ignorar = ObtenerCodigosIgnorados(ignorados);

                DateOnly fechaPago = Formatos.ObtenerFechaHoraLocal();

                porCobrar = await _db.Prestamos.Where(p => p.PagoCompleto == false && p.FechaPago <= fechaPago && p.FechaPago != null && p.IdUsuario == idUsuario && !ignorar.Contains(p.IdDeudor) && p.Propio == false).CountAsync();
                return porCobrar;
            }
            catch (Exception)
            {

                return porCobrar;
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
                    PagoCompleto = false,
                    Propio = false
                });

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajePrestamoHelper.Registrado);
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> ObtenerIgnorados(int idUsuario)
        {
            var respuesta = new GeneralResponse();
            try
            {
                var ignorados = await (from ignorado in _db.Ignorados
                                       join deudor in _db.Deudores on ignorado.IdDeudor equals deudor.Id
                                       where ignorado.IdUsuario == idUsuario
                                       select new CatalogoResponse
                                       {
                                           Codigo = deudor.Id,
                                           Valor = deudor.Nombres.Trim()
                                       }).OrderBy(i => i.Valor)
                                         .ToListAsync();
                if (ignorados.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.NoHayIgnorados);
                }

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, string.Empty, ignorados);
            }
            catch {
                return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajePrestamoHelper.NoHayIgnorados);
            }
        }

        private  static List<int> ObtenerCodigosIgnorados(GeneralResponse respuesta)
        {
            var codigos = new List<int>();

            if (respuesta.Codigo != CodigoRespuesta.Exito)
            {
                return codigos;
            }

            var ignorados = respuesta.Data as List<CatalogoResponse>;

            codigos = ignorados.Select(i => i.Codigo).ToList();

            return codigos;
        }

    }
}
