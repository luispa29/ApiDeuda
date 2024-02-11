using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public class Formatos
    {
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

    }
}
