using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Deudor.Service
{
    public interface IDeudor
    {
        Task<bool> ExisteDeudor(string deudor, string correo);
        
        Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, string correo);
       
        Task<GeneralResponse> RegistrarDeudor(string deudor, string correo);
        
        Task<GeneralResponse> EditarDeudor(string deudor, string correo);
        
        Task<GeneralResponse> CambiarEstadoDeudor(string deudor, string correo);
    }
}
