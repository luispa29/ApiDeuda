using Interfaces.Abono;
using Interfaces.Deudor.Logica;
using Interfaces.Deudor.Service;
using Interfaces.Gasto;
using Interfaces.Prestamo;
using Interfaces.Usuario;
using Interfaces.Usuario.Services;
using Logica.Abono;
using Logica.Deudor;
using Logica.Prestamo;
using Logica.Usuario;
using Servicios.Abono;
using Servicios.Deudor;
using Servicios.Prestamo;
using Servicios.Usuarios;
using Servicios.Gasto;
using Logica.Gasto;
using Interfaces.Presupuesto;
using Servicios.Presupuesto;
using Logica.Presupueso;

namespace ApiDeuda
{
    public static class Dependencias
    {
        public static IServiceCollection AddDependencyDeclaration(this IServiceCollection services)
        {

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            #region Usuario

            services.AddScoped<IUsuario, UsuarioService>();
            services.AddScoped<IUsuarioLogica, UsuarioLogica>();

            #endregion

            #region Deudor

            services.AddScoped<IDeudor, DeudorService>();
            services.AddScoped<IDeudorLogica, DeudorLogica>();

            #endregion

            #region Prestamo

            services.AddScoped<IPrestamo, PrestamoService>();
            services.AddScoped<IPrestamoLogica, PrestamoLogica>();
            services.AddScoped<ICompartirPrestamo, CompartirPrestamoServices>();
            services.AddScoped<ICompartirPrestamoLogica, CompartirPrestamoLogica>();

            #endregion
            
            #region Abono

            services.AddScoped<IAbono, AbonoService>();
            services.AddScoped<IAbonoLogica, AbonoLogica>();

            #endregion

            #region Gasto

            services.AddScoped<IGasto, GastoService>();
            services.AddScoped<IGastoLogica, GastoLogica>();

            #endregion
            
            #region Presupuesto

            services.AddScoped<IPresupuesto, PresupuestoService>();
            services.AddScoped<IPresupuestoLogica, PresupuestoLogica>();

            #endregion

            return services;
        }

        public static string DevolverTokenLimpio(string token)
        {
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            return token;
        }
    }
}
