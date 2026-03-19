namespace _01_PrimoEsempio.Models;

public class Proiezione
{
    public int Id { get; set; }
    public DateTime DataOra { get; set; }
    public decimal Prezzo { get; set; }
    public int FilmId { get; set; }
    public int SalaId { get; set; }

    public Film Film { get; set; } = null!;
    public Sala Sala { get; set; } = null!;
}