﻿using Modelos.Response;

namespace Interfaces.Deudor.Logica
{
    public interface IDeudorLogica
    {
        Task<GeneralResponse> ConsultarDeudores(string token);

        Task<GeneralResponse> RegistrarDeudor(string deudor, string token);

        Task<GeneralResponse> EditarDeudor(CatalogoResponse deudor, string token);

        Task<GeneralResponse> EliminarDeudor(int idDeudor, string token);
    }
}
