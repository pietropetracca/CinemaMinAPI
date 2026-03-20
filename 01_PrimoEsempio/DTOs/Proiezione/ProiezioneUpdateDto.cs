namespace _01_PrimoEsempio.DTOs.Proiezione;

public class ProiezioneUpdateDto
{
    public DateTime DataOra { get; set; }
    public decimal Prezzo { get; set; }
    public int FilmId { get; set; }
    public int SalaId { get; set; }
}