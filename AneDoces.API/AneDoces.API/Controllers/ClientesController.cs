using AneDoces.API.Data;
using AneDoces.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AneDoces.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetClientePorId(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            return Ok(cliente);
        }

        // GET: api/clientes/buscar-nome/Maria
        [HttpGet("buscar-nome/{nome}")]
        public async Task<ActionResult<IEnumerable<Cliente>>> BuscarClientesPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest(new { mensagem = "Informe um nome para busca." });
            }

            var clientes = await _context.Clientes
                .Where(c => c.Nome.Contains(nome))
                .OrderBy(c => c.Nome)
                .ToListAsync();

            if (!clientes.Any())
            {
                return NotFound(new { mensagem = $"Nenhum cliente encontrado com o nome '{nome}'." });
            }

            return Ok(clientes);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
            {
                return BadRequest(new { mensagem = "O nome do cliente é obrigatório." });
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClientePorId), new { id = cliente.Id }, cliente);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest(new { mensagem = "O ID informado não confere com o cliente enviado." });
            }

            if (string.IsNullOrWhiteSpace(cliente.Nome))
            {
                return BadRequest(new { mensagem = "O nome do cliente é obrigatório." });
            }

            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            clienteExistente.Nome = cliente.Nome;
            clienteExistente.Telefone = cliente.Telefone;
            clienteExistente.WhatsApp = cliente.WhatsApp;
            clienteExistente.Endereco = cliente.Endereco;
            clienteExistente.Observacao = cliente.Observacao;

            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Cliente atualizado com sucesso." });
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "Cliente excluído com sucesso." });
        }
    }
}