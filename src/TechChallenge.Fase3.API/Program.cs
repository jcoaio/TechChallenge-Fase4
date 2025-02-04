using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.OpenApi.Models;
using Prometheus;
using TechChallenge.Fase3.Application.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos.Interfaces;
using TechChallenge.Fase3.Infra.Contatos;
using TechChallenge.Fase3.Infra.Utils;
using TechChallenge.Fase3.Infra.Utils.DBContext;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("mysql");

var mensageriaConfig = new
{
    Servidor = builder.Configuration["Mensageria:Servidor"],
    Usuario = builder.Configuration["Mensageria:Usuario"],
    Senha = builder.Configuration["Mensageria:Senha"]
};


// Add services to the container.
builder.Services.AddControllers();
builder.Services.UseHttpClientMetrics();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<DapperContext>();


ConfigurationManager configurationManager = builder.Configuration;

string servidor = mensageriaConfig.Servidor ?? string.Empty;
string usuario = mensageriaConfig.Usuario ?? string.Empty;
string senha = mensageriaConfig.Senha ?? string.Empty;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(servidor, "/", h =>
        {
            h.Username(usuario);
            h.Password(senha);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IMensageriaBus, MensageriaBus>();

builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosAppServico>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());
builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosRepositorio>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());
builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosServico>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(b =>
{
    b.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge.5Nett.API" });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseMetricServer();
app.UseHttpMetrics();

app.UseCors(c =>
{
    c.AllowAnyHeader();
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
