using DBEF.Models;
using Interfaces.Gasto;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using Microsoft.EntityFrameworkCore;
using Modelos.Response.Prestamo;
using Microsoft.Win32;
using Interfaces.Deudor.Service;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;

namespace Servicios.Gasto
{
    public class GastoService(AppDeudaContext db, IOptions<AppSettings> options) : IGasto
    {
        private readonly AppDeudaContext _db = db;
        public async Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            try
            {
                var prestamos = await (from prestamo in _db.Prestamos
                                       join deudor in _db.Deudores on prestamo.IdDeudor equals deudor.Id

                                       where prestamo.IdUsuario == idUsuario &&
                                             ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null) && prestamo.PagoCompleto == false
                                       select new
                                       {
                                           Prestamo = prestamo,
                                           Deudor = deudor,


                                       })
                                            .OrderByDescending(p => p.Prestamo.FechaPrestamo).ThenByDescending(p => p.Prestamo.Id)
                                            .Skip((pagina) * registros)
                                            .Take(registros)
                                            .ToListAsync();

                var prestamoResponses = prestamos.Select(p => new PrestamoResponse
                {
                    Descripcion = p.Deudor == null ? p.Prestamo.Descripcion.Trim() : $"{SetearPrestamo(p.Prestamo.IdDeudor, idUsuario)}{SetearDeudor(p.Deudor.Nombres, p.Prestamo.Propio)}{LLenarDescripcion(p.Prestamo.Descripcion, p.Prestamo.Propio)}",
                    FechaPrestamo = p.Prestamo.FechaPrestamo,
                    Id = p.Prestamo.Id,
                    Prestamo = p.Prestamo.MontoPrestamo,
                    IdDeudor = (bool)p.Prestamo.Propio ? 0 : 1,
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
            catch (Exception)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }

        }



        private static string LLenarDescripcion(string desripcion, bool? propio)
        {
            return (bool)propio && !string.IsNullOrEmpty(desripcion) ? desripcion : !string.IsNullOrEmpty(desripcion) ? $" para {desripcion}" : string.Empty;
        }

        private static string SetearDeudor(string deudor, bool? propio)
        {
            return (bool)propio ? string.Empty : deudor;
        }

        private static string SetearPrestamo(int idDeudor, int idUsuario)
        {
            return idDeudor == idUsuario ? string.Empty : "Prestamo a ";

        }

        public async Task<GeneralResponse> ConsultarTotal(int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            var total = await _db.Prestamos.Where(prestamo => ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null) && prestamo.PagoCompleto == false && prestamo.IdUsuario == idUsuario).SumAsync(p => p.MontoPrestamo);

            return new GeneralResponse
            {
                Codigo = CodigoRespuesta.Exito,
                Total = total
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
                    PagoCompleto = false,
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

        public async Task<GeneralResponse> RptGastos(int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            try
            {
                string[] formats = { "dd/MM/yyyy" };
                var cadena = options.Value.DefaultConnection;
                List<PrestamoResponse> gastos = new();
                using (var conexion = new SqlConnection(cadena))
                {
                    await conexion.OpenAsync();
                    string consulta = @"Select d.Nombres, p.IdDeudor,p.IdUsuario,p.Descripcion,convert(varchar, p.FechaPrestamo,103) FechaPrestamo, p.MontoPrestamo,p.propio  from Prestamos p
                                        inner join Deudores d on p.IdDeudor = d.id
                                        where p.IdUsuario = @idUsuario and p.FechaPrestamo > = @desde and p.FechaPrestamo <= @hasta and p.PagoCompleto = 0
                                        order by  p.FechaPrestamo desc, p.Id desc";
                    using (var command = new SqlCommand(consulta, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        command.Parameters.AddWithValue("@desde", fechaDesde);
                        command.Parameters.AddWithValue("@hasta", fechaHasta);

                        using SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            gastos.Add(new PrestamoResponse
                            {
                                Descripcion = $"{SetearPrestamo(Convert.ToInt32(reader["IdDeudor"].ToString()), idUsuario)}{SetearDeudor(reader["Nombres"].ToString(), Convert.ToBoolean(reader["propio"].ToString()))}{LLenarDescripcion(reader["Descripcion"].ToString(), Convert.ToBoolean(reader["propio"].ToString()))}",
                                FechaPrestamo = Formatos.FormatearFecha(reader["FechaPrestamo"].ToString()),
                                Prestamo = Convert.ToDecimal(reader["MontoPrestamo"])
                            });
                           
                        }
                    }
                    await conexion.CloseAsync();
                }

                if (gastos.Count == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoHayGastos);
                }

                return new GeneralResponse
                {
                    Codigo = CodigoRespuesta.Exito,
                    Data = gastos
                };
            }
            catch (Exception ex)
            {

                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }
    }
}
