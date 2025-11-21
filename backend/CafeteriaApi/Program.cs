using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CafeteriaApi.Data;
using CafeteriaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header usando esquema Bearer.\r\n\r\n Ingresa el token sin la palabra 'Bearer'."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add custom services
builder.Services.AddScoped<IAutenticacionServicio, AutenticacionServicio>();
builder.Services.AddScoped<IProductoServicio, ProductoServicio>();
builder.Services.AddScoped<IMesaServicio, MesaServicio>();
builder.Services.AddScoped<IPedidoServicio, PedidoServicio>();
builder.Services.AddScoped<IReporteServicio, ReporteServicio>();
builder.Services.AddScoped<ICierreCajaServicio, CierreCajaServicio>();

// Add DbContext with environment detection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var environment = builder.Environment.EnvironmentName;
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl) || environment == "Production")
{
    // Use PostgreSQL for Railway/Production
    var postgresConnection = databaseUrl ?? builder.Configuration.GetConnectionString("PostgreSQLConnection");
    
    // Convert Railway DATABASE_URL format if needed
    if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
    {
        var uri = new Uri(databaseUrl);
        var convertedConnection = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true;";
        postgresConnection = convertedConnection;
    }
    
    builder.Services.AddDbContext<CafeteriaDbContext>(options =>
        options.UseNpgsql(postgresConnection));
}
else
{
    // Use SQL Server for local development
    builder.Services.AddDbContext<CafeteriaDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add CORS with environment-specific origins
var allowedOrigins = new List<string>
{
    "http://localhost:5173",
    "http://localhost:3000",
    "http://localhost:5174"
};

// Add production frontend URL if in production
if (environment == "Production")
{
    var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
    if (!string.IsNullOrEmpty(frontendUrl))
    {
        allowedOrigins.Add(frontendUrl);
    }
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins(allowedOrigins.ToArray())
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Add JWT Authentication with environment-specific secret
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? 
                   builder.Configuration.GetSection("JwtSettings")["SecretKey"];
var secretKey = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Production settings
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cafeteria API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");

// Only use HTTPS redirection in development (Railway handles HTTPS)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure port for Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
