namespace AneDoces.App.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Endereco { get; set; }
    public string? Observacao { get; set; }
}