using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Models;

namespace _01_PrimoEsempio.Data;

public class CinemaDbContext : DbContext
{
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }

    public DbSet<Regista> Registi => Set<Regista>();
    public DbSet<Film> Film => Set<Film>();
    public DbSet<Sala> Sale => Set<Sala>();
    public DbSet<Proiezione> Proiezioni => Set<Proiezione>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Regista>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cognome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Nazionalita).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titolo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Genere).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DurataMinuti).IsRequired();
            entity.Property(e => e.Anno).IsRequired();
            entity.Property(e => e.Descrizione).IsRequired();
            entity.HasOne(e => e.Regista)
                  .WithMany(r => r.Film)
                  .HasForeignKey(e => e.RegistaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Capienza).IsRequired();
        });

        modelBuilder.Entity<Proiezione>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DataOra).IsRequired();
            entity.Property(e => e.Prezzo).IsRequired().HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Film)
                  .WithMany(f => f.Proiezioni)
                  .HasForeignKey(e => e.FilmId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Sala)
                  .WithMany(s => s.Proiezioni)
                  .HasForeignKey(e => e.SalaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}