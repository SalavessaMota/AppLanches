using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public OrderDetailsPage(int orderId, decimal totalPrice, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        LblPrecoTotal.Text = totalPrice + " €";

        GetOrderDetails(orderId);
    }

    private async void GetOrderDetails(int orderId)
    {
        try
        {
            //Show loading indicator
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;

            var (orderDetails, errorMessage) = await _apiService.GetOrderDetails(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if(orderDetails is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Could not get order details.", "OK");
            }
            else
            {
                CvPedidoDetalhes.ItemsSource = orderDetails;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not get order details. Try again later", "OK");
        }
        finally
        {
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}