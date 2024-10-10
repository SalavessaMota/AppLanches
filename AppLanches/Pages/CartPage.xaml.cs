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

        bool savedAddress = Preferences.ContainsKey("Address");

        if (savedAddress)
        {
            string name = Preferences.Get("Name", string.Empty);
            string address = Preferences.Get("Address", string.Empty);
            string phone = Preferences.Get("Phone", string.Empty);

            LblEndereco.Text = $"{name}\n{address}\n{phone}";
        }
        else
        {
            LblEndereco.Text = "No address saved";
        }
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


    private async void BtnIncrementar_Clicked(object sender, EventArgs e)
    {
        if(sender is Button button && button.BindingContext is CartItem cartItem)
        {
            cartItem.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "aumentar");
        }
    }



    private async void BtnDecrementar_Clicked(object sender, EventArgs e)
    {
        if(sender is Button button && button.BindingContext is CartItem cartItem)
        {
            if (cartItem.Quantity == 1) return;
            else
            {
                cartItem.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "diminuir");
            }
        }
    }


    private async void BtnDeletar_Clicked(object sender, EventArgs e)
    {
        if(sender is ImageButton button && button.BindingContext is CartItem cartItem)
        {
            bool resposta = await DisplayAlert("Confirm","Are you sure you want to delete this item from cart?","Yes","No");
            if (resposta)
            {
                CartItems.Remove(cartItem);
                UpdateTotalPrice();
                await _apiService.UpdateCartItemQuantity(cartItem.ProductId, "apagar");
            }
        }
    }

    private void BtnEditaEndereco_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {

    }
}