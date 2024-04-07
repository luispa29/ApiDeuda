using DBEF.Models;
using Interfaces.Usuario.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modelos.Response;
using Modelos.Response.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilidades;
using Utilidades.Helper;

namespace Servicios.Usuarios
{
    public class UsuarioService : IUsuario
    {
        private readonly AppDeudaContext _dbEF;
        private readonly AppSettings _appSettings;


        public UsuarioService(AppDeudaContext appDeudaContext, IOptions<AppSettings> appSettings)
        {
            _dbEF = appDeudaContext;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Consulta todos los usuarios que hay en el sistema
        /// </summary>
        /// <param name="pagina">numero de pagina de consulta</param>
        /// <param name="registros">registro que se mostraran por pagina</param>
        /// <param name="token">para confirmar que el usuario sea de tipo existe</param>
        /// <returns></returns>
        public async Task<GeneralResponse> ConsultarUsuarios(int pagina, int registros, string correo)
        {
            GeneralResponse respuesta = new();
            try
            {
                var token = GenerarToken(correo);

                var usuarios = await _dbEF.Usuarios.Select(u => new UsuarioResponse { Correo = u.Correo.Trim() })
                                                   .Skip((pagina) * registros)
                                                   .Take(registros)
                                                   .ToListAsync();

                if (usuarios.Count == 0 && pagina == 0)
                {
                    return Transaccion.Respuesta(CodigoRespuesta.NoExiste, 0, token, MensajesUsuariosHelper.NoHayUsuariosRegistrados);
                }

                respuesta = Transaccion.Respuesta(CodigoRespuesta.Exito, 0, token, string.Empty);
                respuesta.Data = usuarios;

                return respuesta;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task<bool> EsAdmin(string correo)
        {
            bool admin = false;
            try
            {
                admin = await _dbEF.Usuarios.Where(u => u.Correo.Trim() == correo.Trim() && u.Admin).AnyAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return admin;
        }

        public async Task<CatalogoResponse> Login(string correo)
        {
            CatalogoResponse response = new();
            try
            {
                response = await _dbEF.Usuarios.Where(u => u.Correo.Trim() == correo.Trim()).Select(u=> new CatalogoResponse { Codigo = CodigoRespuesta.Existe, Valor = u.CodigoCompartido}) .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }

        public string GenerarToken(string correo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                         new(ClaimTypes.NameIdentifier,correo.Trim()),
                        }
                        ),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(llave), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                //#region Registrar Error

                //var error = Generadores.Error(_servicio, Generadores.ObtenerImprimirNombreDelMetodo(), HelperUsuario.ObtenerToken, ex);
                //await _mongo.RegistrarError(error);
                //#endregion
                throw;
            }
        }

        public string ObtenerCorreoToken(string token)
        {
            string correo = string.Empty;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var llave = Encoding.ASCII.GetBytes(_appSettings.Secreto);

                // Configura la validación del token
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(llave)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                // Accede al ID de usuario del claim "NameIdentifier"
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    correo = userIdClaim.Value;
                }

            }
            catch (Exception)
            {
            }

            return correo;
        }

        public async Task<int> ObtenerId(string correo)
        {
            try
            {
                return await _dbEF.Usuarios.Where(u => u.Correo.Trim() == correo.Trim()).Select(u => u.Id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                return 0;
            }
        }


        public async Task<GeneralResponse> RegistrarUsuario(string correo)
        {
            GeneralResponse respuesta = new();
            try
            {
                var nuevoUsuario = new Usuario
                {
                    Admin = false,
                    Correo = correo.Trim(),
                    CodigoCompartido = Formatos.CodigoCompartido()
                };

                await _dbEF.Usuarios.AddAsync(nuevoUsuario);
                await _dbEF.SaveChangesAsync();

                return respuesta;
            }
            catch (Exception)
            {
                return Transaccion.Respuesta(CodigoRespuesta.Error, 0, string.Empty, MensajeErrorHelperMensajeErrorHelper.OcurrioError);
            }
        }

        public async Task RegistrarCodigoCompartido(string correo)
        {
            try
            {
                var modificar = await _dbEF.Usuarios.Where(u=> u.Correo == correo).FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(modificar?.CodigoCompartido))
                {
                    modificar.CodigoCompartido = Formatos.CodigoCompartido();

                    await _dbEF.SaveChangesAsync();
                }
            }
            catch 
            {

            }
        }
    }
}
