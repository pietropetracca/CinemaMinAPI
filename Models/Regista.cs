namespace _01_PrimoEsempio.Models;

public class Regista
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Nazionalita { get; set; } = string.Empty;

    public ICollection<Film> Film { get; set; } = new List<Film>();
}