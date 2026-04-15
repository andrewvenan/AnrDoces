namespace AneDoces.API.Models
{
    public class Orcamento
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public DateTime DataOrcamento { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pendente";

        public bool ConvertidoEmPedido { get; set; } = false;

        public int? PedidoId { get; set; }

        public Cliente? Cliente { get; set; }
    }
}