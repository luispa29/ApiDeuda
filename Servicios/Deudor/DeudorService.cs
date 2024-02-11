using DBEF.Models;
using Interfaces.Deudor.Service;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Deudor
{
    public class DeudorService(AppDeudaContext db) : IDeudor
    {
        private readonly AppDeudaContext _db = db;
       
        public Task<GeneralResponse> CambiarEstadoDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> ConsultarDeudores(int pagina, int registros, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> EditarDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> RegistrarDeudor(string deudor, string correo)
        {
            throw new NotImplementedException();
        }
    }
}
