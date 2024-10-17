using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesService _favoritesService;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public FavoritesPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _favoritesService = ServiceFactory.CreateFavoritesService();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }

    private async Task GetFavoriteProducts()
    {
        try
        {
            var favoriteProducts = await _favoritesService.ReadAllAsync();

            if(favoriteProducts is null || favoriteProducts.Count == 0)
            {
                CvProdutos.ItemsSource = null; //Clear the list
                LblAviso.IsVisible = true;  //Show the warning label
            }
            else
            {
                CvProdutos.ItemsSource = favoriteProducts;
                LblAviso.IsVisible = false;
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }



    private void CvProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId, currentSelection.Name!, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}