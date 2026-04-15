using AneDoces.App.Pages;

namespace AneDoces.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnClientesClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ClientesPage());
    }

    private async void OnPedidosClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PedidosPage());
    }

    private async void OnOrcamentosClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new OrcamentosPage());
    }

    private async void OnBuscarClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new BuscaPage());
    }
}