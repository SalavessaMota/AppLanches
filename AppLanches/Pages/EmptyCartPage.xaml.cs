namespace AppLanches.Pages;

public partial class EmptyCartPage : ContentPage
{
	public EmptyCartPage()
	{
		InitializeComponent();
	}

    private async void BtnRetornar_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}