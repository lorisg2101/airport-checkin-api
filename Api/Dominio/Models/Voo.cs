namespace AirportCheckin.Models;

public class Voo
{
    public int Id { get; set; }
    public string NumeroVoo { get; set; } = string.Empty;
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public DateTime DataPartida { get; set; }
    public DateTime? DataChegada { get; set; }

    public ICollection<Cliente>? Passageiros { get; set; }
}