using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirportCheckin.Data;
using AirportCheckin.Models;

namespace AirportCheckin.Endpoints;

public static class VooEndpoints
{
    public static void MapVooEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/voos").WithTags("Voos").RequireAuthorization();

        group.MapGet("/", async ([FromServices] AppDbContext db) => await db.Voos.ToListAsync());

        group.MapGet("/{id:int}", async (int id, [FromServices] AppDbContext db) =>
            await db.Voos.FindAsync(id) is Voo voo ? Results.Ok(voo) : Results.NotFound());

        group.MapPost("/", async (Voo voo, [FromServices] AppDbContext db) =>
        {
            db.Voos.Add(voo);
            await db.SaveChangesAsync();
            return Results.Created($"/voos/{voo.Id}", voo);
        });

        group.MapPut("/{id:int}", async (int id, Voo updated, [FromServices] AppDbContext db) =>
        {
            var voo = await db.Voos.FindAsync(id);
            if (voo is null) return Results.NotFound();

            voo.NumeroVoo = updated.NumeroVoo;
            voo.Origem = updated.Origem;
            voo.Destino = updated.Destino;
            voo.DataPartida = updated.DataPartida;
            voo.DataChegada = updated.DataChegada;

            await db.SaveChangesAsync();
            return Results.Ok(voo);
        });

        group.MapDelete("/{id:int}", async (int id, [FromServices] AppDbContext db) =>
        {
            var voo = await db.Voos.FindAsync(id);
            if (voo is null) return Results.NotFound();
            db.Voos.Remove(voo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
