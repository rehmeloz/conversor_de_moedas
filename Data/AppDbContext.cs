namespace ConversorMoedas.Data;
using Microsoft.EntityFrameworkCore;
using ConversorMoedas.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Moeda> Moedas {  get; set; }
    public DbSet<Conversao> Conversoes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Moeda>().HasData(
            new Moeda { Id = 1, Codigo = "USD", Nome = "Dólar Americano", Taxa = 1.0m, UltimaAtualizacao = DateTime.Now },
            new Moeda { Id = 2, Codigo = "BRL", Nome = "Real Brasileiro", Taxa = 5.0m, UltimaAtualizacao = DateTime.Now },
            new Moeda { Id = 3, Codigo = "EUR", Nome = "Euro", Taxa = 0.92m, UltimaAtualizacao = DateTime.Now });
    }
}
