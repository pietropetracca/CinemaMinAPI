namespace _01_PrimoEsempio.Models;

public class Sala
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Capienza { get; set; }

    public ICollection<Proiezione> Proiezioni { get; set; } = new List<Proiezione>();
}