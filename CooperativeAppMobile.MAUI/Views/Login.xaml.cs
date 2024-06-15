using CommunityToolkit.Maui.Alerts;
using System.Net.Http;
using System.Net;

namespace CooperativeAppMobile.MAUI.Views;

public partial class Login : ContentPage
{
    private APIService service { get; set; }

    public Login()
    {
        InitializeComponent();
        service = new APIService();

    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        activityInd.IsVisible = true;activityInd.IsRunning = true;
        body.IsVisible = false;
        footer.IsVisible = false;
        var session = new tblSessionLog();

        if (txt_user.Text == string.Empty || txt_pass.Text == string.Empty)
        {
            lbl_error.IsVisible = true;
            lbl_error.Text = "Empty username or password not allowed. \n To access the system, pls enter an authorized username & password.";
            return;
        }
        var user_dto = new
        {
            username = txt_user.Text,
            password = txt_pass.Text,
        };
        var response = await service.SignIn(user_dto);

        if (response.Status == 200)
        {
            var splited_response = response.Result.Split(":");
            if (!string.IsNullOrEmpty(await SecureStorage.Default.GetAsync("user_id")))
            { SecureStorage.Default.Remove("user_id");  }
            else { await SecureStorage.Default.SetAsync("user_id", splited_response[1]); }
            activityInd.IsVisible = false; activityInd.IsRunning = false;

            await Shell.Current.GoToAsync($"//{nameof(Dashboard)}");

        }
        else
        {
            lbl_error.Text = response.Result.ToString();
            activityInd.IsVisible = false;
            activityInd.IsRunning = false;
            body.IsVisible = true;
            footer.IsVisible = true;
        }

    }

    private async void Register_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(Register)}");
    }
}