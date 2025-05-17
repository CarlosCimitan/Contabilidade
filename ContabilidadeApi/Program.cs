using ContabilidadeApi.Data;
using ContabilidadeApi.Scalar;
using ContabilidadeApi.Services.AuthService;
using ContabilidadeApi.Services.EmpresaService;
using ContabilidadeApi.Services.SenhaService;
using ContabilidadeApi.Services.UsuarioService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()      // Permite qualquer origem
              .AllowAnyHeader()      // Permite qualquer cabeçalho
              .AllowAnyMethod();     // Permite qualquer método (GET, POST, etc)
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAuth, AuthService>();
builder.Services.AddScoped<ISenha, SenhaService>();
builder.Services.AddScoped<IUsuario, UsuarioService>();
builder.Services.AddScoped<IEmpresa, EmpresaService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostgresDb"));
});

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateAudience = false,
        ValidateIssuer = false,
    };
});
    

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(option =>
    {
        option.Theme = ScalarTheme.Saturn;
    });
    app.MapOpenApi();
}

app.UseCors("PermitirTudo");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
