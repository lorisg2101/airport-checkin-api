namespace AirportCheckin.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public int VooId { get; set; }
    public Voo? Voo { get; set; }
    public decimal ValorPassagem { get; set; }
}