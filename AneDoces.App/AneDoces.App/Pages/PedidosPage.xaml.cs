using AneDoces.App.Models;
using AneDoces.App.Services;

namespace AneDoces.App.Pages;

public partial class PedidosPage : ContentPage
{
    private readonly ApiService _apiService;

    public PedidosPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPedidosAsync();
    }

    private async Task CarregarPedidosAsync()
    {
        try
        {
            var pedidos = await _apiService.GetPedidosAsync();
            PedidosCollectionView.ItemsSource = pedidos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os pedidos.\n{ex.Message}", "OK");
        }
    }

    private async void OnPedidoSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Pedido pedido)
        {
            await DisplayAlert(
                "Pedido",
                $"Cliente: {pedido.Cliente?.Nome}\nDescrição: {pedido.Descricao}\nValor: R$ {pedido.Valor:F2}\nStatus: {pedido.Status}\nData: {pedido.DataPedido:dd/MM/yyyy}",
                "OK");

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}