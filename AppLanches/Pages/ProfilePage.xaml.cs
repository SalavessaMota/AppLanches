using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public ProfilePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblNomeUsuario.Text = Preferences.Get("UserName", string.Empty);
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnPerfil.Source = await GetProfileImage();
    }


    private async Task<string?> GetProfileImage()
    {
        // obtain default image from appconfig
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "Could not get the image.", "Ok");
                    return defaultImage;
            }
        }
        if(response?.UrlImage is not null)
        {
            return response.PathImage;
        }
        return defaultImage;
    }

    private async void ImgBtnPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imagemArray = await SelectImageAsync();
            if (imagemArray is null)
            {
                await DisplayAlert("Error", "Could not load the image", "Ok");
                return;
            }
            ImgBtnPerfil.Source = ImageSource.FromStream(() => new MemoryStream(imagemArray));

            var response = await _apiService.UploadUserImage(imagemArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image sent successfully", "Ok");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "Unexpected error occurred", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Unexpected error occurred: {ex.Message}", "Ok");
        }
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var archive = await MediaPicker.PickPhotoAsync();

            if (archive is null) return null;

            using (var stream = await archive.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "Feature not supported", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permission denied", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error selecting image: {ex.Message}", "Ok");
        }
        return null;    
    }

    private void TapPedidos_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OrdersPage(_apiService, _validator));
    }

    private void MinhaConta_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void Perguntas_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}