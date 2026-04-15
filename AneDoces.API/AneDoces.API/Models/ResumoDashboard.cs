namespace AneDoces.API.Models
{
    public class ResumoDashboard
    {
        public int TotalClientes { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalPedidosAprovados { get; set; }
        public int TotalPedidosReprovados { get; set; }
        public int TotalOrcamentos { get; set; }
        public int TotalOrcamentosPendentes { get; set; }
        public int TotalOrcamentosAprovados { get; set; }
        public int TotalOrcamentosReprovados { get; set; }
        public decimal ValorTotalPedidosAprovados { get; set; }
        public decimal ValorTotalOrcamentosPendentes { get; set; }
    }
}