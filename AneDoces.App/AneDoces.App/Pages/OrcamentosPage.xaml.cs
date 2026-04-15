using AneDoces.App.Models;
using AneDoces.App.Services;

namespace AneDoces.App.Pages;

public partial class OrcamentosPage : ContentPage
{
    private readonly ApiService _apiService;

    public OrcamentosPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarOrcamentosAsync();
    }

    private async Task CarregarOrcamentosAsync()
    {
        try
        {
            var orcamentos = await _apiService.GetOrcamentosAsync();
            OrcamentosCollectionView.ItemsSource = orcamentos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os orçamentos.\n{ex.Message}", "OK");
        }
    }

    private async void OnOrcamentoSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Orcamento orcamento)
        {
            await DisplayAlert(
                "Orçamento",
                $"Cliente: {orcamento.Cliente?.Nome}\nDescrição: {orcamento.Descricao}\nValor: R$ {orcamento.Valor:F2}\nStatus: {orcamento.Status}",
                "OK");

            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnWhatsAppClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Orcamento orcamento)
        {
            var link = await _apiService.GetLinkWhatsAppOrcamentoAsync(orcamento.Id);

            if (string.IsNullOrWhiteSpace(link))
            {
                await DisplayAlert("Aviso", "Não foi possível gerar o link do WhatsApp para este orçamento.", "OK");
                return;
            }

            await Launcher.OpenAsync(link);
        }
    }
}