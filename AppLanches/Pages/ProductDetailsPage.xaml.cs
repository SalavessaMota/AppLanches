using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private int _productId;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

	public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _productId = productId;
        _apiService = apiService;
        _validator = validator;
        Title = productName ?? "Product Details";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
    }

    private async Task<Product> GetProductDetails(int productId)
    {
        var (produtoDetalhe, errorMessage) = await _apiService.GetProductDetails(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (produtoDetalhe == null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Erro", errorMessage ?? "Could not get the product.", "OK");
            return null;
        }

        if (produtoDetalhe != null)
        {
            // Atualizar as propriedades dos controles com os dados do produto
            ImagemProduto.Source = produtoDetalhe.PathImage;
            LblProdutoNome.Text = produtoDetalhe.Name;
            LblProdutoPreco.Text = produtoDetalhe.Price.ToString();
            LblProdutoDescricao.Text = produtoDetalhe.Details;
            LblPrecoTotal.Text = produtoDetalhe.Price.ToString();
        }
        else
        {
            await DisplayAlert("Erro", errorMessage ?? "Could not get the product details.", "OK");
            return null;
        }
        return produtoDetalhe;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void ImagemBtnFavorito_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            // Decrementa a quantidade, e n o permite que seja menor que 1
            quantidade = Math.Max(1, quantidade - 1);
            LblQuantidade.Text = quantidade.ToString();

            // Calcula o pre o total
            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inv lidos", "OK");
        }
    }

    private void BtnAdiciona_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
      decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            // Incrementa a quantidade
            quantidade++;
            LblQuantidade.Text = quantidade.ToString();

            // Calcula o pre o total
            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString(); // Formata como moeda
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inv lidos", "OK");
        }
    }

    private async void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {
        try
        {
            var cart = new Cart()
            {
                Quantity = Convert.ToInt32(LblQuantidade.Text),
                UnitPrice = Convert.ToDecimal(LblProdutoPreco.Text),
                TotalValue = Convert.ToDecimal(LblPrecoTotal.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("UserId", 0)
            };
            var response = await _apiService.AddItemToCart(cart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to cart !", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", $"Error adding item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"An error occurred: {ex.Message}", "OK");
        }
    }
}