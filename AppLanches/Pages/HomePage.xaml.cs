using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;

    public HomePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblNomeUsuario.Text = "Welcome, " + Preferences.Get("UserName", string.Empty);
        _apiService = apiService;
        _validator = validator;
        Title = AppConfig.HomePageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }
        //await GetCategoryList();
        //await GetMostSold();
        //await GetPopular();
    }

    private async Task LoadDataAsync()
    {
        var categoryListTask = GetCategoryList();
        var mostSoldTask = GetMostSold();
        var popularTask = GetPopular();

        await Task.WhenAll(categoryListTask, mostSoldTask, popularTask);
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
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection == null)
            return;

        Navigation.PushAsync(new ProductListPage(currentSelection.Id, currentSelection.Name!,_apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void CvMaisVendidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private void CvPopulares_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(sender is CollectionView collectionView)
        {
            NavigateToProductDetailsPage(collectionView, e);
        }
    }
    private void NavigateToProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null)
            return;

        Navigation.PushAsync(new ProductDetailsPage(
                                 currentSelection.Id, currentSelection.Name!, _apiService, _validator
        ));

        collectionView.SelectedItem = null;
    }

    
}