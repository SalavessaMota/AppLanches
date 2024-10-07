using AppLanches.Services;

namespace AppLanches.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RegisterPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    private async void BtnSignup_ClickedAsync(object sender, EventArgs e)
    {
        var response = await _apiService.RegisterUser(EntNome.Text, EntEmail.Text,
                                                          EntPhone.Text, EntPassword.Text);

        if (!response.HasError)
        {
            await DisplayAlert("Warning", "Account registered successfully !!", "OK");
            await Navigation.PushAsync(new LoginPage(_apiService));
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong!!!", "Cancel");
        }
    }

    private async void TapLogin_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService));
    }
}