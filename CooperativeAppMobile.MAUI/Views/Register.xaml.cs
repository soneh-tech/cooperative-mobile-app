namespace CooperativeAppMobile.MAUI.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();
	}

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
		await Shell.Current.GoToAsync($"//{nameof(Login)}");
    }
  
    private void Register_Clicked(object sender, EventArgs e)
    {

    }
}