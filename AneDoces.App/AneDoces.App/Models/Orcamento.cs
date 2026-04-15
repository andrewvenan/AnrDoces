namespace AneDoces.App.Models;

public class Orcamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataOrcamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool ConvertidoEmPedido { get; set; }
    public int? PedidoId { get; set; }
    public Cliente? Cliente { get; set; }
}