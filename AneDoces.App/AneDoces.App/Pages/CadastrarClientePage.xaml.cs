using AneDoces.App.Models;
using AneDoces.App.Services;

namespace AneDoces.App.Pages;

public partial class CadastrarClientePage : ContentPage
{
    private readonly ApiService _apiService;

    public CadastrarClientePage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnSalvarClienteClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text))
        {
            await DisplayAlert("Aviso", "Informe o nome do cliente.", "OK");
            return;
        }

        var cliente = new Cliente
        {
            Nome = NomeEntry.Text.Trim(),
            Telefone = TelefoneEntry.Text?.Trim(),
            WhatsApp = WhatsAppEntry.Text?.Trim(),
            Endereco = EnderecoEntry.Text?.Trim(),
            Observacao = ObservacaoEditor.Text?.Trim()
        };

        var sucesso = await _apiService.CriarClienteAsync(cliente);

        if (!sucesso)
        {
            await DisplayAlert("Erro", "Não foi possível salvar o cliente.", "OK");
            return;
        }

        await DisplayAlert("Sucesso", "Cliente cadastrado com sucesso.", "OK");
        await Navigation.PopAsync();
    }
}