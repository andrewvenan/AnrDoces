using AneDoces.App.Models;
using AneDoces.App.Services;

namespace AneDoces.App.Pages;

public partial class ClientesPage : ContentPage
{
    private readonly ApiService _apiService;

    public ClientesPage()
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
        try
        {
            var clientes = await _apiService.GetClientesAsync();
            ClientesCollectionView.ItemsSource = clientes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os clientes.\n{ex.Message}", "OK");
        }
    }

    private async void OnNovoClienteClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastrarClientePage());
    }

    private async void OnClienteSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Cliente cliente)
        {
            await DisplayAlert(
                "Cliente",
                $"Nome: {cliente.Nome}\nTelefone: {cliente.Telefone}\nWhatsApp: {cliente.WhatsApp}",
                "OK");

            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnWhatsAppClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Cliente cliente)
        {
            var numero = new string((cliente.WhatsApp ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            if (string.IsNullOrWhiteSpace(numero))
            {
                await DisplayAlert("Aviso", "Cliente sem número de WhatsApp válido.", "OK");
                return;
            }

            if (!numero.StartsWith("55"))
            {
                numero = "55" + numero;
            }

            var url = $"https://wa.me/{numero}";
            await Launcher.OpenAsync(url);
        }
    }
}