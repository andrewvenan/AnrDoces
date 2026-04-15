using AneDoces.API.Data;
using AneDoces.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuscaGeralController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BuscaGeralController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/buscageral/Maria
        [HttpGet("{termo}")]
        public async Task<ActionResult<BuscaGeralResultado>> BuscarGeral(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return BadRequest(new { mensagem = "Informe um termo para busca." });
            }

            termo = termo.Trim();

            var clientes = await _context.Clientes
                .Where(c => c.Nome.Contains(termo))
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.Cliente != null && p.Cliente.Nome.Contains(termo))
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Where(o => o.Cliente != null && o.Cliente.Nome.Contains(termo))
                .OrderByDescending(o => o.DataOrcamento)
                .ToListAsync();

            if (!clientes.Any() && !pedidos.Any() && !orcamentos.Any())
            {
                return NotFound(new
                {
                    mensagem = $"Nenhum resultado encontrado para '{termo}'."
                });
            }

            var resultado = new BuscaGeralResultado
            {
                TermoBuscado = termo,
                Clientes = clientes,
                Pedidos = pedidos,
                Orcamentos = orcamentos
            };

            return Ok(resultado);
        }
    }
}