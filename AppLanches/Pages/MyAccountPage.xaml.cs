using AppLanches.Services;

namespace AppLanches.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string UserNameKey = "UserName";
    private const string UserEmailKey = "Email";
    private const string UserPhoneKey = "Phone";

    public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();
        ImgBtnPerfil.Source = await GetProfileImageAsync();
    }

    private void LoadUserInfo()
    {
        LblNomeUsuario.Text = Preferences.Get(UserNameKey, string.Empty);
        EntNome.Text = LblNomeUsuario.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntFone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async Task<string?> GetProfileImageAsync()
    {
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetProfileImage();

        if(errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "You are not authorized to view this page.", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Could not get the image.", "OK");
                    return defaultImage;
            }
        }
        if(response?.UrlImage is not null)
        {
            return response.PathImage;
        }
        return defaultImage;
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, EntNome.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntFone.Text);
        await DisplayAlert("Success", "Your information has been saved.", "OK");
    }
}