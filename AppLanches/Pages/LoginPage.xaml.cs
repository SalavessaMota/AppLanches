using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public LoginPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Insert your email", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Error", "Insert your password", "Cancel");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong!!!", "Cancel");
        }
    }

    private async void TapRegister_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_apiService, _validator));
    }
}