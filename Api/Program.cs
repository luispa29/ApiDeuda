using ApiDeuda;
using DBEF.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Utilidades;

var builder = WebApplication.CreateBuilder(args);
string MiCors = "MiCors";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configuración de Cors para Usar el APi en Frontend

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MiCors, builder =>
    {
        builder.WithHeaders("*");
        builder.WithOrigins("*");
        builder.WithMethods("*");
        builder.WithExposedHeaders("*");
    });
});

#endregion

#region Conexion Base de Datos

builder.Services.AddDbContext<AppDeudaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("AppSettings").GetSection("DefaultConnection").Value);

});

#endregion

#region Token

var appSettingsSection = builder.Configuration.GetSection("AppSettings");


builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.Configure<AppSettings>(appSettingsSection);

//JWT
var appSettings = appSettingsSection.Get<AppSettings>();

var llave = Encoding.ASCII.GetBytes(appSettings.Secreto);

builder.Services.AddAuthentication(d =>
{

    d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(d =>
    {
        d.RequireHttpsMetadata = false;
        d.SaveToken = true;
        d.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(llave),
            ValidateIssuer = false,
            ValidateAudience = false
        };

    });

#endregion

Dependencias.AddDependencyDeclaration(builder.Services);

IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
builder.Host.UseSerilog();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors(MiCors);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
