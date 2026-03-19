namespace _01_PrimoEsempio.DTOs.Proiezione;

public class ProiezioneReadDto
{
    public int Id { get; set; }
    public DateTime DataOra { get; set; }
    public decimal Prezzo { get; set; }
    public int FilmId { get; set; }
    public string FilmTitolo { get; set; } = string.Empty;
    public int SalaId { get; set; }
    public string SalaNome { get; set; } = string.Empty;
    public int PostiDisponibili { get; set; }
}