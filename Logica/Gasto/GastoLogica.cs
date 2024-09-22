
using Interfaces.Gasto;
using Interfaces.Prestamo;
using Interfaces.Usuario.Services;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Utilidades.Helper;
using Utilidades;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using Modelos.Response.Prestamo;
using Modelos.Response.Gasto;
using Interfaces.Presupuesto;

namespace Logica.Gasto
{
    public class GastoLogica(IGasto _gasto, IUsuario _usuario, IPresupuesto _presupuesto) : IGastoLogica
    {
    
        public async Task<GeneralResponse> Consultar(int pagina, int registros, string token, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                token = _usuario.GenerarToken(correo);

                int idUsuario = await _usuario.ObtenerId(correo);

                DateOnly? fechaDesdeConsulta = null;
                DateOnly? fechaHastaConsulta = null;

                if (fechaDesde != null && fechaHasta != null)
                {
                    fechaDesdeConsulta = Formatos.DevolverSoloFecha((DateTime)fechaDesde);
                    fechaHastaConsulta = Formatos.DevolverSoloFecha((DateTime)fechaHasta);
                }

                GeneralResponse consulta = await _gasto.ConsultarGastos(pagina, registros, idUsuario, fechaDesdeConsulta, fechaHastaConsulta);

                if (consulta.Codigo == CodigoRespuesta.Exito && pagina == 0)
                {
                    GeneralResponse totalGasto = await _gasto.ConsultarTotal(idUsuario, fechaDesdeConsulta, fechaHastaConsulta);
                    consulta.Total = totalGasto.Total;
                }
                consulta.Token = token;
                return consulta;

            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Editar(PrestamoQuery gasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existe = await _gasto.Existe(gasto.Id, idUsuario);

                if (!existe) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajeGastoHelper.NoExiste); }

                GeneralResponse editar = await _gasto.Editar(gasto, idUsuario);
                editar.Token = token;

                return editar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Eliminar(int idGasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                bool existe = await _gasto.Existe(idGasto, idUsuario);

                if (!existe) { return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajeGastoHelper.NoExiste); }

                GeneralResponse eliminar = await _gasto.Eliminar(idGasto, idUsuario);
                eliminar.Token = token;

                return eliminar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> Registrar(PrestamoQuery gasto, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                int idUsuario = await _usuario.ObtenerId(correo);

                token = _usuario.GenerarToken(correo);

                GeneralResponse registrar = await _gasto.Registrar(gasto, idUsuario);
                registrar.Token = token;
                return registrar;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> RptGasto(string token, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                MemoryStream stream = new();
                string correo = _usuario.ObtenerCorreoToken(token);
                token = _usuario.GenerarToken(correo);

                int idUsuario = await _usuario.ObtenerId(correo);

                DateOnly fechaDesdeConsulta = Formatos.DevolverSoloFecha((DateTime)fechaDesde); ;
                DateOnly fechaHastaConsulta = Formatos.DevolverSoloFecha((DateTime)fechaHasta);

                var respuesta = await _gasto.RptGastos(idUsuario, fechaDesdeConsulta, fechaHastaConsulta);
                respuesta.Token = token;
                if (respuesta.Codigo == CodigoRespuesta.Exito)
                {
                    List<PrestamoResponse> data = respuesta.Data as List<PrestamoResponse>;

                    Document document = new();
                    Section seccion1 = document.AddSection();

                    var gasto = data.Sum(d => d.Prestamo);

                    PdfCabecera(ref seccion1, fechaDesdeConsulta, fechaHastaConsulta, gasto);
                    PdfCuerpo(ref seccion1, data);

                    PdfDocumentRenderer renderer = new()
                    {
                        Document = document
                    };

                    renderer.RenderDocument();
                    renderer.PdfDocument.Save(stream, false);
                    stream.Position = 0;

                    return new GeneralResponse
                    {
                        Codigo = CodigoRespuesta.Exito,
                        Data = Convert.ToBase64String(stream.ToArray()),
                        Token = token
                    };
                }
                else
                {
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        private static void PdfCabecera(ref Section section, DateOnly desde, DateOnly hasta, decimal gasto)
        {
            Paragraph cabecera = section.AddParagraph();
            cabecera.Format.Alignment = ParagraphAlignment.Center;
            cabecera.Format.Font.Name = "Verdana";
            cabecera.Format.Font.Bold = true;
            cabecera.Format.Font.Size = 14;
            cabecera.AddText(MensajeGastoHelper.RptNombre);
            cabecera.AddLineBreak();
            cabecera.AddLineBreak();
            cabecera.AddLineBreak();


            Table tablaTotal = section.AddTable();
            tablaTotal.AddColumn(40);
            tablaTotal.AddColumn(400);

            Table tablaDesde = section.AddTable();
            tablaDesde.AddColumn(40);
            tablaDesde.AddColumn(400);

            Table tablaHasta = section.AddTable();
            tablaHasta.AddColumn(40);
            tablaHasta.AddColumn(400);

            Row filaGasto = tablaTotal.AddRow();
            Row filaDesde = tablaDesde.AddRow();
            Row filaHasta = tablaHasta.AddRow();

            AgregarCelda(ref filaGasto, "Gasto:", 0, true, false);
            AgregarCelda(ref filaGasto, $"${gasto}", 1, false, true);

            AgregarCelda(ref filaDesde, "Desde:", 0, true, false);
            AgregarCelda(ref filaDesde, desde.ToShortDateString(), 1, false, true);

            AgregarCelda(ref filaHasta, "Hasta:", 0, true, false);
            AgregarCelda(ref filaHasta, hasta.ToString(), 1, false, true);

        }

        private static void AgregarCelda(ref Row fila, string valor, int columna, bool bold, bool ultimo)
        {

            Paragraph parrafo = new();
            parrafo.Format.Font.Bold = bold;
            parrafo.Format.Font.Size = 12;
            parrafo.Format.Alignment = ParagraphAlignment.Left;
            FormattedText texto = new();

            texto.AddText(valor);
            parrafo.Add(texto);
            if (ultimo)
            {
                parrafo.AddLineBreak();
                parrafo.AddLineBreak();
            }
            fila.Cells[columna].Add(parrafo);
        }

        private static void PdfCuerpo(ref Section section, List<PrestamoResponse> data)
        {
            Table tabla = section.AddTable();
            tabla.AddColumn(70);
            tabla.AddColumn(335);
            tabla.AddColumn(70);

            Row cabecera = tabla.AddRow();
            cabecera.Format.Shading.Color = new MigraDoc.DocumentObjectModel.Color(66, 140, 255);
            cabecera.Format.Font.Size = 14;
            cabecera.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
            cabecera.Format.Font.Bold = true;
            cabecera.Format.Alignment = ParagraphAlignment.Center;
            cabecera.HeadingFormat = true;
            cabecera.Cells[0].AddParagraph("Fecha");
            cabecera.Cells[1].AddParagraph("Descripción");
            cabecera.Cells[1].Format.LeftIndent = -20;
            cabecera.Cells[2].AddParagraph("Valor");
            cabecera.Cells[2].Format.LeftIndent = -20;

            foreach (var gasto in data)
            {
                Row fila = tabla.AddRow();
                AgregarCelda(ref fila, gasto.FechaPrestamo.ToString("dd/MM/yyy"), 0, false, false);
                AgregarCelda(ref fila, gasto.Descripcion.Trim(), 1, false, false);
                AgregarCelda(ref fila, $"${gasto.Prestamo}", 2, false, false);
            }
        }

        public async Task<GeneralResponse> ResumenGastos(string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);

                token = _usuario.GenerarToken(correo);

                int idUsuario = await _usuario.ObtenerId(correo);

                DateOnly fechaHastaConsulta = Formatos.ObtenerFechaHoraLocal();
                DateOnly fechaDesdeConsulta = Formatos.DevolverPrimerDiaMes(fechaHastaConsulta);
                decimal presupuesto = await _presupuesto.Obtener(idUsuario, fechaDesdeConsulta.Month, fechaDesdeConsulta.Year);

                var respuesta = await _gasto.RptGastos(idUsuario, fechaDesdeConsulta, fechaHastaConsulta);

                respuesta.Token = token;

                if (respuesta.Codigo != CodigoRespuesta.Exito) { respuesta.Data = presupuesto; return respuesta; }

                List<PrestamoResponse>? data = respuesta.Data as List<PrestamoResponse>;

                ResumenResponse resumen = new()
                {
                    Prestamo = data?.Where(d => !d.Propio).Sum(d => d.Prestamo),
                    Propio = data?.Where(d => d.Propio).Sum(d => d.Prestamo),
                    Presupuesto = presupuesto,
                    Resumen =
                            [.. data?
                                 .GroupBy(p => p.Descripcion)
                                 .Select(g => new Resumen { Descripcion = g.Key,
                                                            Total = g.Sum(p => p.Prestamo)
                                                          }
                                        )
                                 .OrderByDescending(d => d.Total)
                            ]
                };
                respuesta.Data = resumen;

                return respuesta;
            }

            catch { return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError); }
        }
    }
}
