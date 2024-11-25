using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TechChallenge.Fase3.Application.Contatos.Servicos;
using TechChallenge.Fase3.Domain.Contatos.Servicos;
using TechChallenge.Fase3.Infra.Contatos;
using TechChallenge.Fase3.Infra.Utils.DBContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<DapperContext>();

builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosAppServico>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());
builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosRepositorio>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());
builder.Services.Scan(scan => scan.FromAssemblyOf<ContatosServico>().AddClasses().AsImplementedInterfaces().WithScopedLifetime());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSwaggerGen(b =>
{
    b.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge.5Nett.API" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

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