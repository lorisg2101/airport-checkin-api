using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AirportCheckin.Data;
using AirportCheckin.Endpoints;


var builder = WebApplication.CreateBuilder(args);


// Configurar DB (MySQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "server=localhost;database=airportcheckin;user=root;password=123456";
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// JWT configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASampleSecretForDevReplaceIt";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "airport-checkin";
var audience = builder.Configuration["Jwt:Audience"] ?? "airport-checkin-audience";


builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
  };
});


builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();


// Map endpoints
app.MapAdminEndpoints();
app.MapVooEndpoints();
app.MapClienteEndpoints();


app.Run();