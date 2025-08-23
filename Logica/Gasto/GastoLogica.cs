
using DocumentFormat.OpenXml.Bibliography;
using Interfaces.Gasto;
using Interfaces.Prestamo;
using Interfaces.Presupuesto;
using Interfaces.Usuario.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Modelos.Query.Prestamo;
using Modelos.Response;
using Modelos.Response.Gasto;
using Modelos.Response.Prestamo;
using MongoDB.Driver.Linq;
using System.Data;
using Utilidades;
using Utilidades.Helper;

namespace Logica.Gasto
{
    public class GastoLogica(IGasto _gasto, IUsuario _usuario, IPresupuesto _presupuesto, ILogger<GastoLogica> _logger) : IGastoLogica
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<GeneralResponse> CargaMasiva(IFormFile file, string token)
        {
            try
            {
                string correo = _usuario.ObtenerCorreoToken(token);
                token = _usuario.GenerarToken(correo);

                int idUsuario = await _usuario.ObtenerId(correo);


                if (file == null || file.Length == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.Error, 0, token, MensajeGastoHelper.SinArchivo);
                }

                //ClosedXML.Excel.IXLWorksheet worksheet = await ObtenerArchivo(file);

                using MemoryStream stream = new();
                await file.CopyToAsync(stream);

                using ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(stream);
                ClosedXML.Excel.IXLWorksheet worksheet = workbook.Worksheets.First();
                List<string> cabecera = worksheet.Row(1).Cells().Select(c => c.GetString().Trim()).ToList();

                if (!string.IsNullOrEmpty(ValidarColumnas(cabecera)))
                {
                    return Transaccion.Respuesta(CodigoRespuesta.Error, 0, token, MensajeGastoHelper.ArchivoIncorrecto);
                }

                List<string> errores = new();

                #pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
                IEnumerable<ClosedXML.Excel.IXLRangeRow> filas = worksheet.RangeUsed().RowsUsed().Skip(1);
                #pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

                DataTable gastos = FormatearTablaGastos();

                int rowIndex = 2;

                foreach (var fila in filas)
                {
                    string validar = ValidarFila(fila, cabecera);
                    if (string.IsNullOrEmpty(validar))
                    {
                        gastos.Rows.Add(SetearDatosFila(gastos, fila, idUsuario, cabecera));
                    }
                    else
                    {
                        errores.Add($"Error en la fila {rowIndex}: {validar}");
                    }
                }

                if (errores.Count > 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.Error, 0, token, MensajeGastoHelper.RevisarArchivo, errores);
                }

                GeneralResponse registrar = await _gasto.CargaMasiva(gastos);
                registrar.Token = token;
                return registrar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
                _logger.LogError(ex, ex.Message);
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

        private static void AgregarCelda(ref MigraDoc.DocumentObjectModel.Tables.Row fila, string valor, int columna, bool bold, bool ultimo)
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

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }


        private static string ValidarColumnas(List<string> cabeceraArhivo)
        {
            List<string> cabecera = ["Fecha", "Descripcion", "Valor", "Tarjeta"];

            bool valido = cabecera.SequenceEqual(cabeceraArhivo);

            return valido ? string.Empty : MensajeGastoHelper.ArchivoIncorrecto;
        }

        private static string ValidarFila(ClosedXML.Excel.IXLRangeRow fila, List<string> cabecera)
        {
            string error = string.Empty;
            string fecha = fila.Cell(cabecera.IndexOf("Fecha") + 1).GetString().Trim();
            string descripcion = fila.Cell(cabecera.IndexOf("Descripcion") + 1).GetString().Trim();
            string valor = fila.Cell(cabecera.IndexOf("Valor") + 1).GetString().Trim();
            string[] datos = [fecha, descripcion, valor];
            bool vacios = datos.Where(d => string.IsNullOrEmpty(d)).Any();

            if (vacios)
            {
                error = "Por favor llenar todos los campos";
            }
            if (!DateTime.TryParse(fila.Cell(cabecera.IndexOf("Fecha") + 1).GetString(), out DateTime fechaValida))
            {
                error += " Por favor ingresar una fecha valida";
            }
            if (!decimal.TryParse(fila.Cell(cabecera.IndexOf("Valor") + 1).GetString(), out decimal valorValido))
            {
                error += "Por favor ingresar un valor valido";
            }

            return error;
        }

        private static DataTable FormatearTablaGastos()
        {
            string[] campos = Formatos.ObtenerColumnasGastos();
            List<KeyValuePair<string, string>> lista = [];

            for (int i = 0; i < campos.Length; i++)
            {
                string tipo = string.Empty;
                switch (i)
                {
                    case 0:
                    case 1:
                        tipo = "int";
                        break;
                    case 2:
                        tipo = "string";
                        break;
                    case 3:
                        tipo = "dateonly";
                        break;
                    case 4:
                        tipo = "decimal";
                        break;
                    case 5:
                    case 6:
                        tipo = "bool";
                        break;
                }
                lista.Add(new KeyValuePair<string, string>(campos[i], tipo));
            }
            return Formatos.CrearTabla(lista);
        }

        private static DataRow SetearDatosFila(DataTable dataTable, ClosedXML.Excel.IXLRangeRow fila, int idUsuario, List<string> cabecera)
        {
            // Crear un nuevo DataRow a partir del DataTable
            DataRow dataRow = dataTable.NewRow();
            string descripcion = fila.Cell(cabecera.IndexOf("Descripcion") + 1).GetString().Trim();
            string tarjeta = fila.Cell(cabecera.IndexOf("Tarjeta") + 1).GetString().Trim();
            DateOnly fecha = Formatos.DevolverSoloFecha(DateTime.Parse(fila.Cell(cabecera.IndexOf("Fecha") + 1).GetString().Trim()));
            decimal valor = decimal.Parse(fila.Cell(cabecera.IndexOf("Valor") + 1).GetString().Trim());
            dataRow["IdDeudor"] = idUsuario;
            dataRow["IdUsuario"] = idUsuario;
            dataRow["Descripcion"] = tarjeta.ToLower() == "si" ? $"Tarjeta - {descripcion}" : descripcion;
            dataRow["FechaPrestamo"] = fecha;
            dataRow["MontoPrestamo"] = valor;
            dataRow["PagoCompleto"] = false;
            dataRow["Propio"] = true;

            return dataRow;
        }

        private static async Task<ClosedXML.Excel.IXLWorksheet> ObtenerArchivo(IFormFile file)
        {
            using MemoryStream stream = new();
            await file.CopyToAsync(stream);

            using ClosedXML.Excel.XLWorkbook workbook = new ClosedXML.Excel.XLWorkbook(stream);
            ClosedXML.Excel.IXLWorksheet worksheet = workbook.Worksheets.First();

            return worksheet;
        }
    }
}
