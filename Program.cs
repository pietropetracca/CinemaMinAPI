using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Models;

var builder = WebApplication.CreateBuilder(args);

var baseConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Port=3306;Database=cinemaAI;User=root;";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "root";
var connectionString = $"{baseConnection}Password={password};";

builder.Services.AddDbContext<CinemaDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
    db.Database.EnsureCreated();
    SeedData.Seed(db);
}

app.MapRegistaRoutes();
app.MapFilmRoutes();
app.MapSalaRoutes();
app.MapProiezioneRoutes();

app.Run();

public static class SeedData
{
    public static void Seed(CinemaDbContext db)
    {
        if (db.Registi.Any()) return;

        var regista1 = new Regista { Nome = "Christopher", Cognome = "Nolan", Nazionalita = "Britannico" };
        var regista2 = new Regista { Nome = "Quentin", Cognome = "Tarantino", Nazionalita = "Americano" };
        var regista3 = new Regista { Nome = "Greta", Cognome = "Gerwig", Nazionalita = "Americano" };
        var regista4 = new Regista { Nome = "Denis", Cognome = "Villeneuve", Nazionalita = "Canadese" };

        db.Registi.AddRange(regista1, regista2, regista3, regista4);
        db.SaveChanges();

        var film1 = new Film { Titolo = "Oppenheimer", Genere = "Biografico", DurataMinuti = 180, Anno = 2023, Descrizione = "La storia del padre della bomba atomica", RegistaId = regista1.Id };
        var film2 = new Film { Titolo = "Interstellar", Genere = "Sci-Fi", DurataMinuti = 169, Anno = 2014, Descrizione = "Viaggio attraverso i buchi neri", RegistaId = regista1.Id };
        var film3 = new Film { Titolo = "Dune: Parte Due", Genere = "Sci-Fi", DurataMinuti = 166, Anno = 2024, Descrizione = "La lotta per Arrakis", RegistaId = regista4.Id };
        var film4 = new Film { Titolo = "Barbie", Genere = "Commedia", DurataMinuti = 114, Anno = 2023, Descrizione = "Viaggio nel mondo delle bambole", RegistaId = regista3.Id };
        var film5 = new Film { Titolo = "Pulp Fiction", Genere = "Thriller", DurataMinuti = 154, Anno = 1994, Descrizione = "Storie interconnesse nel mondo criminale", RegistaId = regista2.Id };

        db.Film.AddRange(film1, film2, film3, film4, film5);
        db.SaveChanges();

        var sala1 = new Sala { Nome = "Sala 1", Capienza = 100 };
        var sala2 = new Sala { Nome = "Sala 2", Capienza = 80 };
        var sala3 = new Sala { Nome = "Sala 3", Capienza = 120 };

        db.Sale.AddRange(sala1, sala2, sala3);
        db.SaveChanges();

        var proiezione1 = new Proiezione { DataOra = DateTime.Today.AddDays(1).AddHours(18), Prezzo = 12.00m, FilmId = film1.Id, SalaId = sala1.Id };
        var proiezione2 = new Proiezione { DataOra = DateTime.Today.AddDays(1).AddHours(21), Prezzo = 12.00m, FilmId = film2.Id, SalaId = sala1.Id };
        var proiezione3 = new Proiezione { DataOra = DateTime.Today.AddDays(2).AddHours(19), Prezzo = 14.00m, FilmId = film3.Id, SalaId = sala2.Id };
        var proiezione4 = new Proiezione { DataOra = DateTime.Today.AddDays(3).AddHours(16), Prezzo = 10.00m, FilmId = film4.Id, SalaId = sala3.Id };
        var proiezione5 = new Proiezione { DataOra = DateTime.Today.AddDays(4).AddHours(20), Prezzo = 11.00m, FilmId = film5.Id, SalaId = sala2.Id };

        db.Proiezioni.AddRange(proiezione1, proiezione2, proiezione3, proiezione4, proiezione5);
        db.SaveChanges();
    }
}