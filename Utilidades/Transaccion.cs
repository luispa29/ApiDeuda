using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public class Transaccion
    {
        /// <summary>
        /// Devuelve la respuesta de la transaccion
        /// </summary>
        /// <param name="codigo">codigo de la transaccion</param>
        /// <param name="contador">contador de alerta</param>
        /// <param name="token">refrezcar token</param>
        /// <param name="mensaje">mensaje de respuesta</param>
        /// <returns></returns>
        public static GeneralResponse Respuesta(int codigo, int contador, string token, string mensaje)
        {
            return new GeneralResponse
            {
                Codigo = codigo,
                Contador = contador,
                Token = token,
                Mensaje = mensaje
            };
        }

    }

}
