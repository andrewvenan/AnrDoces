namespace AneDoces.App.Models;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPedido { get; set; }
    public string Status { get; set; } = string.Empty;
    public Cliente? Cliente { get; set; }
}