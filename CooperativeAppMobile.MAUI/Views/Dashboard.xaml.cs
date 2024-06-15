
namespace CooperativeAppMobile.MAUI.Views;

public partial class Dashboard : ContentPage
{
    private APIService service { get; set; }
    public Dashboard()
    {
        InitializeComponent();
        service = new APIService();
    }
    protected override void OnAppearing()
    {
        PopulateDasboard();
        indicator.IsVisible = false;
        items_panel.IsVisible = true;
    }
    private async void PopulateDasboard()
    {
        var daily_loan = await service.DailyLoan();
        txt_daily_loan.Text = string.Format("{0:C}", daily_loan);
        var total_loan = await service.TotalLoans();
        txt_total_loan.Text = string.Format("{0:C}", total_loan);

        var daily_deposit = await service.DailyDeposit();
        txt_daily_deposit.Text = string.Format("{0:C}", daily_deposit);
        var total_deposit = await service.TotalDeposits();
        txt_total_deposit.Text = string.Format("{0:C}", total_deposit);

        var daily_withdrawal = await service.DailyWithdrawals();
        txt_daily_withdrawal.Text = string.Format("{0:C}", daily_withdrawal);
        var total_withdrawal = await service.TotalWithdrawals();
        txt_total_withdrawal.Text = string.Format("{0:C}", total_withdrawal);


        var daily_members = await service.DailyMembers();
        txt_daily_member.Text = string.Format("{0:N}", Convert.ToDecimal(daily_members));
        txt_daily_member.Text = txt_daily_member.Text.Replace(".00", "");

        var total_members = await service.TotalMembers();
        txt_total_members.Text = string.Format("{0:N}", Convert.ToDecimal(total_members));
        txt_total_members.Text = txt_total_members.Text.Replace(".00", "");
        //txt_daily_deposit.Text = string.Format("{0:N}", (await service.DailyDeposit() > 0 ? $"$ {await service.DailyDeposit()}" : $"$ {0.00}"));
        //txt_daily_loan.Text = Convert.ToString(await service.DailyLoan() > 0 ? $"$ {await service.DailyLoan()}" : $"$ {0.00}");
        //txt_daily_member.Text = Convert.ToString(await service.DailyMembers() > 0 ? $"{await service.DailyMembers()}" : $"{0}");
        //txt_daily_withdrawal.Text = Convert.ToString(await service.DailyWithdrawals() > 0 ? $"$ {await service.DailyWithdrawals()}" : $"$ {0.00}");
        //txt_total_deposit.Text = Convert.ToString(await service.TotalDeposits() > 0 ? $"$ {await service.TotalDeposits()}" : $"$ {0.00}");
        //txt_total_loan.Text = Convert.ToString(await service.TotalLoans() > 0 ? $"$ {await service.TotalLoans()}" : $"$ {0.00}");
        //txt_total_members.Text = Convert.ToString(await service.TotalMembers() > 0 ? $"{await service.TotalMembers()}" : $"{0}");
        //txt_total_withdrawal.Text = Convert.ToString(await service.TotalWithdrawals() > 0 ? $"$ {await service.TotalWithdrawals()}" : $"$ {0.00}");
    }
    private async void btn_logout_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(Login)}");
    }
}