using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblNomeUsuario.Text = "Welcome, " + Preferences.Get("UserName", string.Empty);
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoryList();
        await GetMostSold();
        await GetPopular();
    }

    private async Task<IEnumerable<Category>> GetCategoryList()
    {
        try
        {
            var (categorias, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categorias == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Could not get categories.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategorias.ItemsSource = categorias;
            return categorias;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetMostSold()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProducts("maisvendido", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Could not get products.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvMaisVendidos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }

    }

    private async Task<IEnumerable<Product>> GetPopular()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Could not get products.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvPopulares.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void CvPopulares_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}