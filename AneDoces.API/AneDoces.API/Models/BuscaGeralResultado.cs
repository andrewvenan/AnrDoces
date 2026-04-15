namespace AneDoces.API.Models
{
    public class BuscaGeralResultado
    {
        public string TermoBuscado { get; set; } = string.Empty;

        public List<Cliente> Clientes { get; set; } = new();

        public List<Pedido> Pedidos { get; set; } = new();

        public List<Orcamento> Orcamentos { get; set; } = new();
    }
}