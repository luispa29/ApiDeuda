using Modelos.Response;

namespace Interfaces.Presupuesto
{
    public interface IPresupuesto
    {
        Task<bool> ExistePresupuesto(int usuarioId, int mes, int anio);
        Task<GeneralResponse> Registrar(int usuarioId, int mes, int anio, decimal preupuesto);
    }
}
