using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using AirportCheckin.Data;
using AirportCheckin.Models;

namespace AirportCheckin.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/admin").WithTags("Administradores");

        group.MapPost("/register", async ([FromServices] AppDbContext db, AdministradorCreateRequest req) =>
        {
            if (await db.Administradores.AnyAsync(a => a.Email == req.Email))
                return Results.Conflict(new { message = "Email já cadastrado" });

            var admin = new Administrador
            {
                Nome = req.Nome,
                Email = req.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha)
            };

            db.Administradores.Add(admin);
            await db.SaveChangesAsync();
            return Results.Created($"/admin/{admin.Id}", admin);
        });

        group.MapPost("/login", async ([FromServices] AppDbContext db, LoginRequest req, IConfiguration config) =>
        {
            var admin = await db.Administradores.FirstOrDefaultAsync(a => a.Email == req.Email);
            if (admin is null) return Results.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(req.Senha, admin.SenhaHash)) return Results.Unauthorized();

            var jwtKey = config["Jwt:Key"] ?? "ThisIsASampleSecretForDevReplaceIt";
            var issuer = config["Jwt:Issuer"] ?? "airport-checkin";
            var audience = config["Jwt:Audience"] ?? "airport-checkin-audience";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()), new Claim(ClaimTypes.Name, admin.Nome), new Claim(ClaimTypes.Email, admin.Email) }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Results.Ok(new { token = jwt });
        });

        group.MapGet("/seed", [AllowAnonymous] async ([FromServices] AppDbContext db) =>
        {
            // cria admin default se não existir
            if (!await db.Administradores.AnyAsync())
            {
                var admin = new Administrador { Nome = "Admin Root", Email = "admin@airport.local", SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123") };
                db.Administradores.Add(admin);
            }

            // cria alguns voos
            if (!await db.Voos.AnyAsync())
            {
                db.Voos.AddRange(
                    new Voo { NumeroVoo = "AB1234", Origem = "GRU", Destino = "JFK", DataPartida = DateTime.UtcNow.AddDays(1) },
                    new Voo { NumeroVoo = "CD5678", Origem = "GIG", Destino = "LHR", DataPartida = DateTime.UtcNow.AddDays(2) }
                );
            }

            await db.SaveChangesAsync();
            return Results.Ok(new { message = "Seed executado" });
        });
    }

    public record AdministradorCreateRequest(string Nome, string Email, string Senha);
    public record LoginRequest(string Email, string Senha);
}