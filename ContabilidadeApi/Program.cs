using ContabilidadeApi.Data;
using ContabilidadeApi.Services.AuthServices;
using ContabilidadeApi.Services.CodigoServices;
using ContabilidadeApi.Services.CodigoServices.Interfaces;
using ContabilidadeApi.Services.ContaContabilServices;
using ContabilidadeApi.Services.EmpresaServices;
using ContabilidadeApi.Services.EnumServices;
using ContabilidadeApi.Services.HistoricoServices;
using ContabilidadeApi.Services.LancamentoContabeisServices;
using ContabilidadeApi.Services.RelatorioServices;
using ContabilidadeApi.Services.SenhaService;
using ContabilidadeApi.Services.UsuarioServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddScoped<ISenha, SenhaService>();
builder.Services.AddScoped<IUsuario, UsuarioService>();
builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddScoped<IEmpresa, EmpresaService>();
builder.Services.AddScoped<IContaContabil, ContaContabilService>();
builder.Services.AddScoped<ILancamentoContabil, LancamentoContabilService>();
builder.Services.AddScoped<IHistorico, HistoricoService>();
builder.Services.AddScoped<IRelatorio, RelatorioService>();
builder.Services.AddScoped<ICodigoService, CodicoService>();
builder.Services.AddScoped<IEnumService, EnumService>();

builder.Services.AddHttpContextAccessor();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
        };


    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1"));


}


app.UseHttpsRedirection();

app.UseCors("PermitirTudo");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
