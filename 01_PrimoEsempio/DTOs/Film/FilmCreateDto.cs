namespace _01_PrimoEsempio.DTOs.Film;

public class FilmCreateDto
{
    public string Titolo { get; set; } = string.Empty;
    public string Genere { get; set; } = string.Empty;
    public int DurataMinuti { get; set; }
    public int Anno { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public int RegistaId { get; set; }
}