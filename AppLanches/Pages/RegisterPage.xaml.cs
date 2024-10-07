//using Android.Service.Autofill;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public RegisterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignup_ClickedAsync(object sender, EventArgs e)
    {
        if (await _validator.ValidateAsync(EntNome.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text))
        {
            var response = await _apiService.RegisterUser(EntNome.Text, EntEmail.Text,
                                                           EntPhone.Text, EntPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Warning", "Account registered successfully !!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong!!!", "Cancel");
            }
        }
        else
        {
            string errorMsg = "";
            errorMsg += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            errorMsg += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            errorMsg += _validator.PhoneError != null ? $"\n- {_validator.PhoneError}" : "";
            errorMsg += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMsg, "Ok");
        }
    }

    private async void TapLogin_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}