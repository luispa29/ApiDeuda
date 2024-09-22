using Modelos.Response;

namespace Interfaces.Presupuesto
{
    public interface IPresupuesto
    {
        Task<decimal> Obtener(int usuarioId, int mes, int anio);
        Task<GeneralResponse> Registrar(int usuarioId, int mes, int anio, decimal preupuesto);
        Task<GeneralResponse> Actualizar (int usuarioId, int mes, int anio, decimal preupuesto);
    }
}
