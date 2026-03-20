using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.Models;

namespace _01_PrimoEsempio.Tests.Helpers;

public static class TestDataBuilder
{
    public static RegistaCreateDto RegistaCreate() => new()
    {
        Nome = "Test",
        Cognome = "Regista",
        Nazionalita = "Italiano"
    };

    public static RegistaUpdateDto RegistaUpdate() => new()
    {
        Nome = "Aggiornato",
        Cognome = "Regista",
        Nazionalita = "Italiano"
    };

    public static FilmCreateDto FilmCreate(int registaId) => new()
    {
        Titolo = "Film Test",
        Genere = "Azione",
        DurataMinuti = 120,
        Anno = 2024,
        Descrizione = "Descrizione test",
        RegistaId = registaId
    };

    public static FilmUpdateDto FilmUpdate(int registaId) => new()
    {
        Titolo = "Film Aggiornato",
        Genere = "Commedia",
        DurataMinuti = 100,
        Anno = 2024,
        Descrizione = "Descrizione aggiornata",
        RegistaId = registaId
    };

    public static SalaCreateDto SalaCreate() => new()
    {
        Nome = "Sala Test",
        Capienza = 100
    };

    public static SalaUpdateDto SalaUpdate() => new()
    {
        Nome = "Sala Aggiornata",
        Capienza = 150
    };

    public static ProiezioneCreateDto ProiezioneCreate(int filmId, int salaId) => new()
    {
        DataOra = DateTime.Today.AddDays(1).AddHours(20),
        Prezzo = 10.00m,
        FilmId = filmId,
        SalaId = salaId
    };

    public static ProiezioneUpdateDto ProiezioneUpdate(int filmId, int salaId) => new()
    {
        DataOra = DateTime.Today.AddDays(2).AddHours(21),
        Prezzo = 12.00m,
        FilmId = filmId,
        SalaId = salaId
    };

    public static Regista RegistaEntity() => new()
    {
        Nome = "Test",
        Cognome = "Regista",
        Nazionalita = "Italiano"
    };

    public static Film FilmEntity(int registaId) => new()
    {
        Titolo = "Film Test",
        Genere = "Azione",
        DurataMinuti = 120,
        Anno = 2024,
        Descrizione = "Descrizione test",
        RegistaId = registaId
    };

    public static Sala SalaEntity() => new()
    {
        Nome = "Sala Test",
        Capienza = 100
    };

    public static Proiezione ProiezioneEntity(int filmId, int salaId) => new()
    {
        DataOra = DateTime.Today.AddDays(1).AddHours(20),
        Prezzo = 10.00m,
        FilmId = filmId,
        SalaId = salaId
    };
}
