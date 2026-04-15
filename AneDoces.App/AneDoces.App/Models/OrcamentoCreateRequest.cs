namespace AneDoces.App.Models;

public class OrcamentoCreateRequest
{
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
}