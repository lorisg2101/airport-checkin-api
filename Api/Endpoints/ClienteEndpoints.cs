using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirportCheckin.Data;
using AirportCheckin.Models;

namespace AirportCheckin.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/clientes").WithTags("Clientes").RequireAuthorization();

        group.MapGet("/", async ([FromServices] AppDbContext db) => await db.Clientes.Include(c => c.Voo).ToListAsync());

        group.MapGet("/{id:int}", async (int id, [FromServices] AppDbContext db) =>
            await db.Clientes.Include(c => c.Voo).FirstOrDefaultAsync(c => c.Id == id) is Cliente cliente ? Results.Ok(cliente) : Results.NotFound());

        group.MapPost("/", async (ClienteCreateRequest req, [FromServices] AppDbContext db) =>
        {
            // checar voo
            var voo = await db.Voos.FindAsync(req.VooId);
            if (voo is null) return Results.BadRequest(new { message = "Voo inválido" });

            // checar CPF duplicado
            if (await db.Clientes.AnyAsync(c => c.CPF == req.CPF)) return Results.Conflict(new { message = "CPF já cadastrado" });

            var cliente = new Cliente
            {
                Nome = req.Nome,
                CPF = req.CPF,
                VooId = req.VooId,
                ValorPassagem = req.ValorPassagem
            };

            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
            return Results.Created($"/clientes/{cliente.Id}", cliente);
        });

        group.MapPut("/{id:int}", async (int id, ClienteUpdateRequest req, [FromServices] AppDbContext db) =>
        {
            var cliente = await db.Clientes.FindAsync(id);
            if (cliente is null) return Results.NotFound();

            if (cliente.CPF != req.CPF && await db.Clientes.AnyAsync(c => c.CPF == req.CPF))
                return Results.Conflict(new { message = "CPF em uso por outro cliente" });

            var voo = await db.Voos.FindAsync(req.VooId);
            if (voo is null) return Results.BadRequest(new { message = "Voo inválido" });

            cliente.Nome = req.Nome;
            cliente.CPF = req.CPF;
            cliente.VooId = req.VooId;
            cliente.ValorPassagem = req.ValorPassagem;

            await db.SaveChangesAsync();
            return Results.Ok(cliente);
        });

        group.MapDelete("/{id:int}", async (int id, [FromServices] AppDbContext db) =>
        {
            var cliente = await db.Clientes.FindAsync(id);
            if (cliente is null) return Results.NotFound();
            db.Clientes.Remove(cliente);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    public record ClienteCreateRequest(string Nome, string CPF, int VooId, decimal ValorPassagem);
    public record ClienteUpdateRequest(string Nome, string CPF, int VooId, decimal ValorPassagem);
}
