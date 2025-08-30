using DBEF.Models;
using Interfaces.Gasto;
using Interfaces.Usuario.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Modelos.Response.Prestamo;
using Servicios.Usuarios;
using System.Data;
using Utilidades;
using Utilidades.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Servicios.Gasto
{
    public class GastoService(AppDeudaContext db, IOptions<AppSettings> options, ILogger<UsuarioService> _logger) : IGasto
    {
        private readonly AppDeudaContext _db = db;
        public async Task<GeneralResponse> ConsultarGastos(int pagina, int registros, int idUsuario, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            try
            {
                var prestamos = (from prestamo in _db.Prestamos
                                 join deudor in _db.Deudores on prestamo.IdDeudor equals deudor.Id
                                 join categoria in _db.Catalogos on prestamo.IdCategoria equals categoria.Id into categoriaJoin
                                 from categoria in categoriaJoin.DefaultIfEmpty()
                                 join medio in _db.Catalogos on prestamo.IdMedio equals medio.Id into medioJoin
                                 from medio in medioJoin.DefaultIfEmpty()
                                 where prestamo.IdUsuario == idUsuario &&
                                       ((prestamo.FechaPrestamo >= fechaDesde && prestamo.FechaPrestamo <= fechaHasta) || fechaDesde == null || fechaHasta == null) && prestamo.PagoCompleto == false
                                 select new
                                 {
                                     Prestamo = prestamo,
                                     Deudor = deudor,
                                     categoria,
                                     medio 
                                 })
                                .OrderByDescending(p => p.Prestamo.FechaPrestamo).ThenByDescending(p => p.Prestamo.Id)
                                .Skip((pagina) * registros)
                                .Take(registros);

                var prestamosList = await prestamos.ToListAsync();

                var prestamoResponses = prestamosList.Select(p => new PrestamoResponse
                {
                    Descripcion = p.Deudor == null ? p.Prestamo.Descripcion.Trim() : $"{SetearPrestamo(p.Prestamo.IdDeudor, idUsuario)}{SetearDeudor(p.Deudor.Nombres, p.Prestamo.Propio)}{LLenarDescripcion(p.Prestamo.Descripcion, p.Prestamo.Propio)}",
                    FechaPrestamo = p.Prestamo.FechaPrestamo,
                    Id = p.Prestamo.Id,
                    Prestamo = p.Prestamo.MontoPrestamo,
                    IdDeudor = p.Prestamo.Propio.HasValue && p.Prestamo.Propio.Value ? 0 : 1,
                    Categoria = p.categoria != null ? p.categoria.Nombre : string.Empty,
                    IdCategoria = p.categoria != null && p.categoria.Id != 0 ? p.categoria.Id : 5,
                    IdMedio = p.medio != null && p.medio.Id != 0 ? p.medio.Id : 1,
                    Medio = p.medio != null ? p.medio.Nombre : string.Empty,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                editar.IdCategoria = gasto.IdCategoria;
                editar.IdMedio = gasto.IdMedio;

                await _db.SaveChangesAsync();

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Actualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                    Propio = true,
                    IdCategoria= gasto.IdCategoria,
                    IdMedio= gasto.IdMedio
                }
                     );
                await _db.SaveChangesAsync();
                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Registrado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> CargaMasiva(DataTable gastos)
        {
            try
            {
                var cadena = options.Value.DefaultConnection;
                using (var conexion = new SqlConnection(cadena))
                {
                    await conexion.OpenAsync();
                    using var bulkCopy = new SqlBulkCopy(conexion)
                    {
                        DestinationTableName = "dbo.Prestamos"
                    };

                    string[] campos = Formatos.ObtenerColumnasGastos();

                    foreach (var campo in campos)
                    {
                        bulkCopy.ColumnMappings.Add(campo, campo);
                    }

                    await bulkCopy.WriteToServerAsync(gastos);
                    await conexion.CloseAsync();
                }

                return Transaccion.Respuesta(CodigoRespuesta.Exito, 0, string.Empty, MensajeGastoHelper.Registrado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError,ex);
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
                    string consulta = @"Select 
                                                d.Nombres,
                                                p.IdDeudor,
                                                p.IdUsuario,
                                                LTRIM(RTRIM(
		                                                CASE WHEN m.Nombre IS NOT NULL AND LTRIM(RTRIM(m.Nombre)) <> '' 
                                                             THEN  m.Nombre + ' - ' ELSE '' END +
                                                        CASE WHEN c.Nombre IS NOT NULL AND LTRIM(RTRIM(c.Nombre)) <> '' 
                                                             THEN c.Nombre + ' - ' ELSE '' END 
		                                                + Descripcion
                                                    )) AS Descripcion,
                                                convert(varchar, p.FechaPrestamo,103) FechaPrestamo, 
                                                p.MontoPrestamo,
                                                p.propio 
                                        from Prestamos p
                                        inner join Deudores d on p.IdDeudor = d.id
                                        left join Catalogo c on p.IdCategoria = c.id
                                        left join Catalogo m on p.IdMedio = m.Id
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
                                Prestamo = Convert.ToDecimal(reader["MontoPrestamo"]),
                                Propio = Convert.ToBoolean(reader["propio"].ToString())
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
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> GetCatalogo(string nombre)
        {
            var catalogos = await _db.Catalogos.Where(c=> c.Categoria.ToLower().Trim() == nombre.ToLower().Trim())
                                               .Select(c=> new CatalogoResponse { Codigo = c.Id, Valor = c.Nombre})
                                               .ToListAsync();

            if(catalogos.Count == 0)
            {
                return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, string.Empty, MensajeGastoHelper.NoHayCatalogos);
            }
            return new GeneralResponse
            {
                Codigo = CodigoRespuesta.Exito,
                Data = catalogos
            };
        }
    }
}
