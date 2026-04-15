using AneDoces.App.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace AneDoces.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7263/")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Cliente>> GetClientesAsync()
    {
        try
        {
            var clientes = await _httpClient.GetFromJsonAsync<List<Cliente>>("api/clientes");
            return clientes ?? new List<Cliente>();
        }
        catch
        {
            return new List<Cliente>();
        }
    }

    public async Task<bool> CriarClienteAsync(Cliente cliente)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/clientes", cliente);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Pedido>> GetPedidosAsync()
    {
        try
        {
            var pedidos = await _httpClient.GetFromJsonAsync<List<Pedido>>("api/pedidos");
            return pedidos ?? new List<Pedido>();
        }
        catch
        {
            return new List<Pedido>();
        }
    }

    public async Task<List<Orcamento>> GetOrcamentosAsync()
    {
        try
        {
            var orcamentos = await _httpClient.GetFromJsonAsync<List<Orcamento>>("api/orcamentos");
            return orcamentos ?? new List<Orcamento>();
        }
        catch
        {
            return new List<Orcamento>();
        }
    }

    public async Task<bool> CriarOrcamentoAsync(OrcamentoCreateRequest orcamento)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/orcamentos", orcamento);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AprovarOrcamentoAsync(int orcamentoId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/orcamentos/aprovar/{orcamentoId}", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> AprovarEEnviarOrcamentoAsync(int orcamentoId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/orcamentos/aprovar-enviar/{orcamentoId}", null);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<AprovarEnviarResponse>(json, _jsonOptions);

            return resultado?.LinkWhatsApp;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetLinkWhatsAppOrcamentoAsync(int orcamentoId)
    {
        try
        {
            var resposta = await _httpClient.GetFromJsonAsync<WhatsAppOrcamentoResponse>(
                $"api/orcamentos/whatsapp/{orcamentoId}");

            return resposta?.LinkWhatsApp;
        }
        catch
        {
            return null;
        }
    }

    public async Task<BuscaGeralResultado?> BuscarGeralAsync(string termo)
    {
        try
        {
            var resultado = await _httpClient.GetFromJsonAsync<BuscaGeralResultado>(
                $"api/buscageral/{Uri.EscapeDataString(termo)}");

            return resultado;
        }
        catch
        {
            return null;
        }
    }
}

public class WhatsAppOrcamentoResponse
{
    public string? Cliente { get; set; }
    public string? Telefone { get; set; }
    public string? Mensagem { get; set; }
    public string? LinkWhatsApp { get; set; }
}

public class AprovarEnviarResponse
{
    public string? Mensagem { get; set; }
    public int OrcamentoId { get; set; }
    public int PedidoId { get; set; }
    public string? Cliente { get; set; }
    public string? Telefone { get; set; }
    public string? TextoWhatsApp { get; set; }
    public string? LinkWhatsApp { get; set; }
}