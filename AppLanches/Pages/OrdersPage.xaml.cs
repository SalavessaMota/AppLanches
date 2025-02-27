using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public OrdersPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetOrdersList();
    }

    private async Task GetOrdersList()
    {
        try
        {
            //Show loading indicator
            loadPedidosIndicator.IsRunning = true;
            loadPedidosIndicator.IsVisible = true;

            var (orders, errorMessage) = await _apiService.GetOrdersByUser(Preferences.Get("UserId", 0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Error", "No orders found.", "OK");
                return;
            }
            if(orders is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not get orders.", "OK");
            }
            else
            {
                CvPedidos.ItemsSource = orders;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not get orders. Try again later", "OK");
        }
        finally
        {
            loadPedidosIndicator.IsRunning = false;
            loadPedidosIndicator.IsVisible = false;
        }
    }

    private void CvPedidos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrderByUser;

        if( selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailsPage(selectedItem.Id, selectedItem.Total, _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator));
    }

}