using AneDoces.App.Services;

namespace AneDoces.App.Pages;

public partial class BuscaPage : ContentPage
{
    private readonly ApiService _apiService;

    public BuscaPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        var termo = BuscaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(termo))
        {
            await DisplayAlert("Aviso", "Digite um nome para buscar.", "OK");
            return;
        }

        await BuscarAsync(termo);
    }

    private async Task BuscarAsync(string termo)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            ResultadoTitulo.IsVisible = false;
            ClientesSection.IsVisible = false;
            PedidosSection.IsVisible = false;
            OrcamentosSection.IsVisible = false;

            var resultado = await _apiService.BuscarGeralAsync(termo);

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;

            if (resultado == null)
            {
                await DisplayAlert("Busca", "Nenhum resultado encontrado ou erro ao consultar a API.", "OK");
                return;
            }

            ResultadoTitulo.Text = $"Resultados para: {resultado.TermoBuscado}";
            ResultadoTitulo.IsVisible = true;

            ClientesCollectionView.ItemsSource = resultado.Clientes;
            PedidosCollectionView.ItemsSource = resultado.Pedidos;
            OrcamentosCollectionView.ItemsSource = resultado.Orcamentos;

            ClientesSection.IsVisible = resultado.Clientes.Any();
            PedidosSection.IsVisible = resultado.Pedidos.Any();
            OrcamentosSection.IsVisible = resultado.Orcamentos.Any();

            if (!resultado.Clientes.Any() && !resultado.Pedidos.Any() && !resultado.Orcamentos.Any())
            {
                await DisplayAlert("Busca", "Nenhum resultado encontrado.", "OK");
            }
        }
        catch (Exception ex)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;

            await DisplayAlert("Erro", $"Não foi possível realizar a busca.\n{ex.Message}", "OK");
        }
    }
}