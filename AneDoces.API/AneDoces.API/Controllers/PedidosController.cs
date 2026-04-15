using AneDoces.API.Data;
using AneDoces.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return Ok(pedidos);
        }

        // GET: api/pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedidoPorId(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound(new { mensagem = "Pedido não encontrado." });
            }

            return Ok(pedido);
        }

        // GET: api/pedidos/status/Aprovado
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosPorStatus(string status)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.Status.ToLower() == status.ToLower())
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum pedido encontrado com status '{status}'." });
            }

            return Ok(pedidos);
        }

        // GET: api/pedidos/cliente/1
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosPorCliente(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
            {
                return NotFound(new { mensagem = "Nenhum pedido encontrado para este cliente." });
            }

            return Ok(pedidos);
        }

        // GET: api/pedidos/buscar-cliente-nome/Maria
        [HttpGet("buscar-cliente-nome/{nome}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> BuscarPedidosPorNomeCliente(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest(new { mensagem = "Informe um nome para busca." });
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.Cliente != null && p.Cliente.Nome.Contains(nome))
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum pedido encontrado para clientes com o nome '{nome}'." });
            }

            return Ok(pedidos);
        }

        // POST: api/pedidos
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            if (pedido.ClienteId <= 0)
            {
                return BadRequest(new { mensagem = "O ClienteId é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(pedido.Descricao))
            {
                return BadRequest(new { mensagem = "A descrição do pedido é obrigatória." });
            }

            if (pedido.Valor <= 0)
            {
                return BadRequest(new { mensagem = "O valor do pedido deve ser maior que zero." });
            }

            var cliente = await _context.Clientes.FindAsync(pedido.ClienteId);

            if (cliente == null)
            {
                return BadRequest(new { mensagem = "Cliente não encontrado." });
            }

            pedido.DataPedido = DateTime.Now;

            if (string.IsNullOrWhiteSpace(pedido.Status))
            {
                pedido.Status = "EmAberto";
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedidoPorId), new { id = pedido.Id }, pedido);
        }

        // PUT: api/pedidos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return BadRequest(new { mensagem = "O ID informado não confere com o pedido enviado." });
            }

            if (pedido.ClienteId <= 0)
            {
                return BadRequest(new { mensagem = "O ClienteId é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(pedido.Descricao))
            {
                return BadRequest(new { mensagem = "A descrição do pedido é obrigatória." });
            }

            if (pedido.Valor <= 0)
            {
                return BadRequest(new { mensagem = "O valor do pedido deve ser maior que zero." });
            }

            var pedidoExistente = await _context.Pedidos.FindAsync(id);

            if (pedidoExistente == null)
            {
                return NotFound(new { mensagem = "Pedido não encontrado." });
            }

            var cliente = await _context.Clientes.FindAsync(pedido.ClienteId);

            if (cliente == null)
            {
                return BadRequest(new { mensagem = "Cliente não encontrado." });
            }

            pedidoExistente.ClienteId = pedido.ClienteId;
            pedidoExistente.Descricao = pedido.Descricao;
            pedidoExistente.Valor = pedido.Valor;
            pedidoExistente.Status = string.IsNullOrWhiteSpace(pedido.Status) ? "EmAberto" : pedido.Status;

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Pedido atualizado com sucesso." });
        }

        // PUT: api/pedidos/aprovar/5
        [HttpPut("aprovar/{id}")]
        public async Task<IActionResult> AprovarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new { mensagem = "Pedido não encontrado." });
            }

            pedido.Status = "Aprovado";
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Pedido aprovado com sucesso." });
        }

        // PUT: api/pedidos/reprovar/5
        [HttpPut("reprovar/{id}")]
        public async Task<IActionResult> ReprovarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new { mensagem = "Pedido não encontrado." });
            }

            pedido.Status = "Reprovado";
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Pedido reprovado com sucesso." });
        }

        // DELETE: api/pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new { mensagem = "Pedido não encontrado." });
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Pedido excluído com sucesso." });
        }
    }
}