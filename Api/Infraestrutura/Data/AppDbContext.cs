namespace AirportCheckin.Data;


using Microsoft.EntityFrameworkCore;
using AirportCheckin.Models;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<Administrador> Administradores { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Voo> Voos { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Administrador>().HasIndex(a => a.Email).IsUnique();
        modelBuilder.Entity<Cliente>().HasIndex(c => c.CPF).IsUnique();
        modelBuilder.Entity<Voo>().HasIndex(v => v.NumeroVoo).IsUnique();
    }
}