using AneDoces.API.Data;
using AneDoces.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrcamentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrcamentosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orcamentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orcamento>>> GetOrcamentos()
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .OrderByDescending(o => o.DataOrcamento)
                .ToListAsync();

            return Ok(orcamentos);
        }

        // GET: api/orcamentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orcamento>> GetOrcamentoPorId(int id)
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orcamento == null)
            {
                return NotFound(new { mensagem = "Orçamento não encontrado." });
            }

            return Ok(orcamento);
        }

        // GET: api/orcamentos/status/Pendente
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Orcamento>>> GetOrcamentosPorStatus(string status)
        {
            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Where(o => o.Status.ToLower() == status.ToLower())
                .OrderByDescending(o => o.DataOrcamento)
                .ToListAsync();

            if (!orcamentos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum orçamento encontrado com status '{status}'." });
            }

            return Ok(orcamentos);
        }

        // GET: api/orcamentos/cliente/1
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Orcamento>>> GetOrcamentosPorCliente(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Where(o => o.ClienteId == clienteId)
                .OrderByDescending(o => o.DataOrcamento)
                .ToListAsync();

            if (!orcamentos.Any())
            {
                return NotFound(new { mensagem = "Nenhum orçamento encontrado para este cliente." });
            }

            return Ok(orcamentos);
        }

        // GET: api/orcamentos/buscar-cliente-nome/Maria
        [HttpGet("buscar-cliente-nome/{nome}")]
        public async Task<ActionResult<IEnumerable<Orcamento>>> BuscarOrcamentosPorNomeCliente(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest(new { mensagem = "Informe um nome para busca." });
            }

            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Where(o => o.Cliente != null && o.Cliente.Nome.Contains(nome))
                .OrderByDescending(o => o.DataOrcamento)
                .ToListAsync();

            if (!orcamentos.Any())
            {
                return NotFound(new { mensagem = $"Nenhum orçamento encontrado para clientes com o nome '{nome}'." });
            }

            return Ok(orcamentos);
        }

        // POST: api/orcamentos
        [HttpPost]
        public async Task<ActionResult<Orcamento>> PostOrcamento(Orcamento orcamento)
        {
            if (orcamento.ClienteId <= 0)
                return BadRequest(new { mensagem = "ClienteId obrigatório." });

            if (string.IsNullOrWhiteSpace(orcamento.Descricao))
                return BadRequest(new { mensagem = "Descrição obrigatória." });

            if (orcamento.Valor <= 0)
                return BadRequest(new { mensagem = "Valor deve ser maior que zero." });

            var cliente = await _context.Clientes.FindAsync(orcamento.ClienteId);

            if (cliente == null)
                return BadRequest(new { mensagem = "Cliente não encontrado." });

            orcamento.DataOrcamento = DateTime.Now;
            orcamento.Status = "Pendente";
            orcamento.ConvertidoEmPedido = false;
            orcamento.PedidoId = null;

            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrcamentoPorId), new { id = orcamento.Id }, orcamento);
        }

        // PUT: api/orcamentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrcamento(int id, Orcamento orcamento)
        {
            if (id != orcamento.Id)
                return BadRequest(new { mensagem = "O ID informado não confere com o orçamento enviado." });

            if (orcamento.ClienteId <= 0)
                return BadRequest(new { mensagem = "ClienteId obrigatório." });

            if (string.IsNullOrWhiteSpace(orcamento.Descricao))
                return BadRequest(new { mensagem = "Descrição obrigatória." });

            if (orcamento.Valor <= 0)
                return BadRequest(new { mensagem = "Valor deve ser maior que zero." });

            var orcamentoExistente = await _context.Orcamentos.FindAsync(id);

            if (orcamentoExistente == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            var cliente = await _context.Clientes.FindAsync(orcamento.ClienteId);

            if (cliente == null)
                return BadRequest(new { mensagem = "Cliente não encontrado." });

            orcamentoExistente.Descricao = orcamento.Descricao;
            orcamentoExistente.Valor = orcamento.Valor;
            orcamentoExistente.ClienteId = orcamento.ClienteId;

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Atualizado com sucesso." });
        }

        // DELETE: api/orcamentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrcamento(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);

            if (orcamento == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            _context.Orcamentos.Remove(orcamento);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Excluído com sucesso." });
        }

        // PUT: api/orcamentos/aprovar/5
        [HttpPut("aprovar/{id}")]
        public async Task<IActionResult> AprovarOrcamento(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);

            if (orcamento == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            orcamento.Status = "Aprovado";

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Orçamento aprovado." });
        }

        // PUT: api/orcamentos/reprovar/5
        [HttpPut("reprovar/{id}")]
        public async Task<IActionResult> ReprovarOrcamento(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);

            if (orcamento == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            orcamento.Status = "Reprovado";

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Orçamento reprovado." });
        }

        // GET: api/orcamentos/whatsapp/5
        [HttpGet("whatsapp/{id}")]
        public async Task<IActionResult> GerarWhatsApp(int id)
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orcamento == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            if (orcamento.Cliente == null)
                return BadRequest(new { mensagem = "Cliente não encontrado." });

            if (string.IsNullOrWhiteSpace(orcamento.Cliente.WhatsApp))
                return BadRequest(new { mensagem = "Cliente sem WhatsApp." });

            var numero = new string(orcamento.Cliente.WhatsApp
                .Where(char.IsDigit)
                .ToArray());

            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest(new { mensagem = "Número de WhatsApp inválido." });

            if (!numero.StartsWith("55"))
                numero = "55" + numero;

            var mensagem = $@"Olá, {orcamento.Cliente.Nome}! 😊

Segue seu orçamento:

🍰 {orcamento.Descricao}
💰 R$ {orcamento.Valor:N2}

Aguardamos sua confirmação!

AneDoces 🍰";

            var link = $"https://wa.me/{numero}?text={Uri.EscapeDataString(mensagem)}";

            return Ok(new
            {
                cliente = orcamento.Cliente.Nome,
                telefone = numero,
                mensagem,
                linkWhatsApp = link
            });
        }

        // PUT: api/orcamentos/aprovar-enviar/5
        [HttpPut("aprovar-enviar/{id}")]
        public async Task<IActionResult> AprovarEEnviar(int id)
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orcamento == null)
                return NotFound(new { mensagem = "Orçamento não encontrado." });

            if (orcamento.Cliente == null)
                return BadRequest(new { mensagem = "Cliente não encontrado." });

            if (string.IsNullOrWhiteSpace(orcamento.Cliente.WhatsApp))
                return BadRequest(new { mensagem = "Cliente sem WhatsApp cadastrado." });

            if (orcamento.ConvertidoEmPedido && orcamento.PedidoId.HasValue)
            {
                var numeroExistente = new string(orcamento.Cliente.WhatsApp
                    .Where(char.IsDigit)
                    .ToArray());

                if (!numeroExistente.StartsWith("55"))
                    numeroExistente = "55" + numeroExistente;

                var mensagemExistente = $@"Olá, {orcamento.Cliente.Nome}! 😊

Seu orçamento já foi aprovado anteriormente.

🍰 Orçamento: {orcamento.Descricao}
💰 Valor: R$ {orcamento.Valor:N2}
📌 Pedido já criado: #{orcamento.PedidoId.Value}

Obrigada pela preferência!
AneDoces 🍰";

                var linkExistente = $"https://wa.me/{numeroExistente}?text={Uri.EscapeDataString(mensagemExistente)}";

                return Ok(new
                {
                    mensagem = "Este orçamento já havia sido convertido em pedido anteriormente.",
                    orcamentoId = orcamento.Id,
                    pedidoId = orcamento.PedidoId.Value,
                    cliente = orcamento.Cliente.Nome,
                    telefone = numeroExistente,
                    textoWhatsApp = mensagemExistente,
                    linkWhatsApp = linkExistente
                });
            }

            orcamento.Status = "Aprovado";

            var pedido = new Pedido
            {
                ClienteId = orcamento.ClienteId,
                Descricao = orcamento.Descricao,
                Valor = orcamento.Valor,
                DataPedido = DateTime.Now,
                Status = "Aprovado"
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            orcamento.ConvertidoEmPedido = true;
            orcamento.PedidoId = pedido.Id;

            await _context.SaveChangesAsync();

            var numero = new string(orcamento.Cliente.WhatsApp
                .Where(char.IsDigit)
                .ToArray());

            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest(new { mensagem = "Número de WhatsApp inválido." });

            if (!numero.StartsWith("55"))
                numero = "55" + numero;

            var mensagem = $@"Olá, {orcamento.Cliente.Nome}! 😊

Seu orçamento foi aprovado com sucesso!

🍰 Pedido: {pedido.Descricao}
💰 Valor: R$ {pedido.Valor:N2}
📌 Status: {pedido.Status}
🧾 Número do pedido: #{pedido.Id}

Obrigada pela preferência!
AneDoces 🍰";

            var link = $"https://wa.me/{numero}?text={Uri.EscapeDataString(mensagem)}";

            return Ok(new
            {
                mensagem = "Orçamento aprovado, pedido criado e link do WhatsApp gerado com sucesso.",
                orcamentoId = orcamento.Id,
                pedidoId = pedido.Id,
                cliente = orcamento.Cliente.Nome,
                telefone = numero,
                textoWhatsApp = mensagem,
                linkWhatsApp = link
            });
        }
    }
}