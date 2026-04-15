using AneDoces.App.Models;
using AneDoces.App.Services;
using System.Globalization;

namespace AneDoces.App.Pages;

public partial class CadastrarOrcamentoPage : ContentPage
{
    private readonly ApiService _apiService;
    private List<Cliente> _clientes = new();

    public CadastrarOrcamentoPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarClientesAsync();
    }

    private async Task CarregarClientesAsync()
    {
        _clientes = await _apiService.GetClientesAsync();
        ClientePicker.ItemsSource = _clientes;
        ClientePicker.ItemDisplayBinding = new Binding("Nome");
    }

    private async void OnSalvarOrcamentoClicked(object sender, EventArgs e)
    {
        if (ClientePicker.SelectedItem is not Cliente cliente)
        {
            await DisplayAlert("Aviso", "Selecione um cliente.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(DescricaoEditor.Text))
        {
            await DisplayAlert("Aviso", "Informe a descrição do orçamento.", "OK");
            return;
        }

        var textoValor = (ValorEntry.Text ?? string.Empty).Replace(",", ".");

        if (!decimal.TryParse(textoValor, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor) || valor <= 0)
        {
            await DisplayAlert("Aviso", "Informe um valor válido.", "OK");
            return;
        }

        var request = new OrcamentoCreateRequest
        {
            ClienteId = cliente.Id,
            Descricao = DescricaoEditor.Text.Trim(),
            Valor = valor
        };

        var sucesso = await _apiService.CriarOrcamentoAsync(request);

        if (!sucesso)
        {
            await DisplayAlert("Erro", "Não foi possível salvar o orçamento.", "OK");
            return;
        }

        await DisplayAlert("Sucesso", "Orçamento cadastrado com sucesso.", "OK");
        await Navigation.PopAsync();
    }
}