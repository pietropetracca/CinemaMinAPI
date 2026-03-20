namespace _01_PrimoEsempio.Models;

public class Film
{
    public int Id { get; set; }
    public string Titolo { get; set; } = string.Empty;
    public string Genere { get; set; } = string.Empty;
    public int DurataMinuti { get; set; }
    public int Anno { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public int RegistaId { get; set; }

    public Regista Regista { get; set; } = null!;
    public ICollection<Proiezione> Proiezioni { get; set; } = new List<Proiezione>();
}