using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Collections.ObjectModel;

namespace AppLanches.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;

    private ObservableCollection<CartItem> CartItems = new ObservableCollection<CartItem>();

    public CartPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCartItems();
    }


    private async Task<bool> GetCartItems()
    {
        try
        {
            var usuarioId = Preferences.Get("UserId", 0);
            var (itensCarrinhoCompra, errorMessage) = await
                     _apiService.GetItemsCart(usuarioId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a p?gina de login
                await DisplayLoginPage();
                return false;
            }

            if (itensCarrinhoCompra == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Could not get cart items.", "OK");
                return false;
            }

            CartItems.Clear();
            foreach (var item in itensCarrinhoCompra)
            {
                CartItems.Add(item);
            }

            CvCarrinho.ItemsSource = CartItems;
            UpdateTotalPrice(); // Atualizar o preco total ap?s atualizar os itens do carrinho

            if (!CartItems.Any())
            {
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Unexpected error occurred: {ex.Message}", "OK");
            return false;
        }
    }

    private void UpdateTotalPrice()
    {
        try
        {
            var precoTotal = CartItems.Sum(item => item.Price * item.Quantity);
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"An error occurred while updating total price: {ex.Message}", "OK");
        }

    }
        private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }





    private void BtnDecrementar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrementar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDeletar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditaEndereco_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {

    }
}