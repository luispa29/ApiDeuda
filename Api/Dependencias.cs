using Interfaces.Usuario;
using Interfaces.Usuario.Services;
using Logica.Usuario;
using Servicios.Usuarios;

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

            return services;
        }

        public static string DevolverTokenLimpio(string token)
        {


            // Elimina el prefijo "Bearer " del token si está presente
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length);
            }

            return token;

        }
    }
}
