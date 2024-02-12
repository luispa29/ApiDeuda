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
        Task<int> ExisteDeudor(string deudor, int idUsuario);
        Task<bool> ExisteDeudorId(int idDeudor, int idUsuario);
        
        Task<GeneralResponse> ConsultarDeudores(int idUsuario);
       
        Task<GeneralResponse> RegistrarDeudor(string deudor, int idUsuario);
        
        Task<GeneralResponse> EditarDeudor(string deudor, int idDeudor, int idUsuario);

        Task<GeneralResponse> CambiarEstadoDeudor(int idDeudor, int idUsuario, bool estado);
    }
}
