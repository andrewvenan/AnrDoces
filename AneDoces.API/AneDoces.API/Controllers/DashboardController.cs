using AneDoces.API.Data;
using AneDoces.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard
        [HttpGet]
        public async Task<ActionResult<ResumoDashboard>> GetResumo()
        {
            var resumo = new ResumoDashboard
            {
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                TotalPedidosAprovados = await _context.Pedidos.CountAsync(p => p.Status == "Aprovado"),
                TotalPedidosReprovados = await _context.Pedidos.CountAsync(p => p.Status == "Reprovado"),
                TotalOrcamentos = await _context.Orcamentos.CountAsync(),
                TotalOrcamentosPendentes = await _context.Orcamentos.CountAsync(o => o.Status == "Pendente"),
                TotalOrcamentosAprovados = await _context.Orcamentos.CountAsync(o => o.Status == "Aprovado"),
                TotalOrcamentosReprovados = await _context.Orcamentos.CountAsync(o => o.Status == "Reprovado"),
                ValorTotalPedidosAprovados = await _context.Pedidos
                    .Where(p => p.Status == "Aprovado")
                    .SumAsync(p => (decimal?)p.Valor) ?? 0,
                ValorTotalOrcamentosPendentes = await _context.Orcamentos
                    .Where(o => o.Status == "Pendente")
                    .SumAsync(o => (decimal?)o.Valor) ?? 0
            };

            return Ok(resumo);
        }
    }
}
