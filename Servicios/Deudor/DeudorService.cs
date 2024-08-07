﻿using DBEF.Models;
using Interfaces.Deudor.Service;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;
using Modelos.Response.Deudor;

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

        public async Task<GeneralResponse> ConsultarDeudores(int idUsuario)
        {
            GeneralResponse respuesta = new();
            try
            {
                List<int> IdsPrestamos = [];

                var consulta = await (from deudor in _db.Deudores
                                      join prestamo in _db.Prestamos on deudor.Id equals prestamo.IdDeudor into prestamoGroup
                                      where deudor.IdUsuario == idUsuario && deudor.Estado
                                      select new
                                      {
                                          Deudor = deudor,
                                          Prestamo = prestamoGroup.DefaultIfEmpty().Where(p => p.PagoCompleto == false && p.Propio == false ),
                                      }
                                      )
                                      .OrderBy(d => d.Deudor.Nombres)
                                      .ToListAsync();

                var deudores = consulta.Select(c => new DeudorResponse
                {
                    Id = c.Deudor.Id,
                    TotalPrestamo = c.Prestamo.Sum(p => p?.MontoPrestamo ?? 0),
                    Nombres = c.Deudor.Nombres.Trim()
                }).ToList();

                ;

                if (deudores.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajesDeudorHelper.NoHayDeudoresRegistrados);
                }

                var prestamos = consulta.SelectMany(c=> c.Prestamo.Select(p=> new { IdPrestamo = p.Id, p.IdDeudor })).ToList();
                
                var idPrestramos = prestamos.Select(p=> p.IdPrestamo).ToList();

                var abonosPagados = await _db.Abonos.Where(a => idPrestramos.Contains(a.IdPrestamo)).Select(a => new { a.IdPrestamo, Abono = a.Abono1 }).ToListAsync();

                var abonoAgrupados = (from abono in abonosPagados
                                      join prestamo in prestamos on abono.IdPrestamo equals prestamo.IdPrestamo

                                      select new
                                      {
                                          Abono = abonosPagados.Where(a => a.IdPrestamo == prestamo.IdPrestamo).Select(a => a.Abono).Sum(),
                                          prestamo.IdDeudor
                                      }).Distinct().ToList();

                foreach (var deudor in deudores)
                {
                    if(deudor.TotalPrestamo > 0)
                    {
                        decimal pagado = abonoAgrupados.Where(a => a.IdDeudor == deudor.Id).Select(a=> a.Abono).FirstOrDefault();
                        deudor.TotalPrestamo -= pagado;
                    }
                }

                respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, string.Empty);
                respuesta.Data = deudores;

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
                int existe = await _db.Deudores.Where(d => d.Nombres.ToUpper().Trim() == deudor.ToUpper().Trim() && d.IdUsuario == idUsuario).Select(d => d.Id).FirstOrDefaultAsync();
                return existe;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ExisteDeudorId(int idDeudor, int idUsuario)
        {
            try
            {
                return await _db.Deudores.Where(d => d.Id == idDeudor && d.IdUsuario == idUsuario).AnyAsync();
            }
            catch (Exception)
            {
                return false;
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
