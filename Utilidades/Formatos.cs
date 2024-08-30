using System;
using System.Collections.Generic;
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
            string timeZoneId = "Eastern Standard Time";
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
    }
}
