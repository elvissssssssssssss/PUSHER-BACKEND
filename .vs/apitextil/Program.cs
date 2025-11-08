
// Program.cs (Configuración completa)
using apitextil.Data;

using apitextil.Services;
using apitextil.Services.apitextil.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Console.WriteLine("?? Unhandled Exception: " + e.ExceptionObject);
};


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database
// Database con reintentos habilitados y configuración segura
builder.Services.AddDbContext<EcommerceContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        cs,
        ServerVersion.AutoDetect(cs),
        mySqlOptions =>
        {
            // ?? Reintentos automáticos en fallos transitorios
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });
});
// Repositories
builder.Services.AddScoped<IProductoService, ProductoService>();
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
// Registrar el servicio del carrito
builder.Services.AddScoped<ICartService, CartService>();
// Agregar el servicio de TblEnvios
builder.Services.AddScoped<ITblEnviosService, TblEnviosService>();
// Agregar el servicio de envíos de voleta 
builder.Services.AddScoped<IEnvioService, EnvioService>();
// Program.cs
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddHttpClient<NiubizService>();

builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddHttpClient(); // para llamadas a Nubefact

builder.Services.AddScoped<IAtencionClienteService, AtencionClienteService>();
builder.Services.AddScoped<IMensajeChatService, MensajeChatService>();



// Para permitir archivos grandes (opcional)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});
// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ??
                "TuSupercalifragilisticoSecretoDeAlMenos32Caracteres"))
        };
    });
// Configurar JSON options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Textil",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFlutter");
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
