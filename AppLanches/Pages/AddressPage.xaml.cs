
namespace AppLanches.Pages;

public partial class AddressPage : ContentPage
{
	public AddressPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSavedData();
    }

    private void LoadSavedData()
    {
        if (Preferences.ContainsKey("Name"))
            EntNome.Text = Preferences.Get("Name", string.Empty);

        if (Preferences.ContainsKey("Address"))
            EntEndereco.Text = Preferences.Get("Address", string.Empty);

        if (Preferences.ContainsKey("Phone"))
            EntTelefone.Text = Preferences.Get("Phone", string.Empty);
    }

    private void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("Name", EntNome.Text);
        Preferences.Set("Address", EntEndereco.Text);
        Preferences.Set("Phone", EntTelefone.Text);
        Navigation.PopAsync();
    }
}