using Modelos.Response;


namespace Interfaces.Presupuesto
{
    public interface IPresupuestoLogica
    {
        Task<GeneralResponse> Registrar(string token, decimal presupuesto);
        Task<GeneralResponse> Actualizar(string token, decimal presupuesto);
    }
}
