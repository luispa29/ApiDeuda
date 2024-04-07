using DBEF.Models;
using Interfaces.Prestamo;
using Microsoft.EntityFrameworkCore;
using Modelos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Prestamo
{
    public class CompartirPrestamoServices(AppDeudaContext db, IPrestamo prestamo) : ICompartirPrestamo
    {

        private readonly AppDeudaContext _db = db;
        private readonly IPrestamo _prestamo = prestamo;

        public async Task<int> ValidoCodigo(int idDeudor, string codigoCompartido)
        {
            try
            {
                var consulta = await (from usuario in _db.Usuarios
                                      join deudor in _db.Deudores on usuario.Id equals deudor.IdUsuario
                                      where usuario.CodigoCompartido == codigoCompartido &&
                                            deudor.Id == idDeudor
                                      select new
                                      {
                                          IdUsuario = usuario.Id,
                                      }).FirstOrDefaultAsync();

                return consulta != null ? consulta.IdUsuario : 0;
            }
            catch (Exception)
            {

                return 0;
            }
        }
    }
}
