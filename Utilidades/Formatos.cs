using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public class Formatos
    {
       public static string[] formats = { "dd/MM/yyyy" };

        public static DateOnly ObtenerFechaHoraLocal()
        {
            string timeZoneId = "SA Pacific Standard Time";
            DateTime horaActual = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, timeZoneId);
            return DevolverSoloFecha(horaActual);
        }

        public static DateOnly DevolverSoloFecha(DateTime fecha)
        {
            return new DateOnly(fecha.Year, fecha.Month, fecha.Day);
        }
        public static DateOnly DevolverPrimerDiaMes(DateOnly fecha)
        {
            return new DateOnly(fecha.Year, fecha.Month, 1);
        }

        public static string ReemplazarTexto(string texto, string remplazar, string textoReemplazo)
        {
            texto = texto.Replace(remplazar, textoReemplazo);
            return texto;
        }
        public static DateOnly FormatearFecha (string fecha)
        {
            fecha = ReemplazarTexto(fecha, " 12:00:00 a. m.", string.Empty);
            fecha = ReemplazarTexto(fecha, " 00:00:00 a. m.", string.Empty);
            fecha = ReemplazarTexto(fecha, " 00:00:00", string.Empty);
            return DateOnly.ParseExact(fecha,formats);
        }

        public static string CodigoCompartido()
        {
            var characters = "0123456789";
            var Charsarr = new char[4];
            var random = new Random();

            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }

            var resultString = new String(Charsarr);
            return resultString;

        }

        public static Type ObtenerTipoColumna(string tipo)
        {
            Type tipoColumna;

            switch (tipo.ToLower())
            {
                case "int":
                    tipoColumna = typeof(int);
                    break;
                case "string":
                    tipoColumna = typeof(string);
                    break;
                case "dateonly":
                    tipoColumna = typeof(DateOnly);
                    break;
                case "bool":
                    tipoColumna = typeof(bool);
                    break;
                case "decimal":
                    tipoColumna = typeof(decimal);
                    break;
                default:
                    tipoColumna = typeof(string);
                    break;
            }
            return tipoColumna;
        }
        //            List<string> cabecera = ["Fecha", "Descripcion", "Valor","Tarjeta"];

        public static string[] ObtenerColumnasGastos()
        {
            return ["IdDeudor", "IdUsuario", "Descripcion", "FechaPrestamo", "MontoPrestamo", "PagoCompleto", "propio"];
        }
       
        public static DataTable CrearTabla(List<KeyValuePair<string, string>> columnas) {

            DataTable dataTable = new();

            foreach(var columna in columnas)
            {
                dataTable.Columns.Add(columna.Key, ObtenerTipoColumna(columna.Value));
            }

            return dataTable;
        }
    }
}
