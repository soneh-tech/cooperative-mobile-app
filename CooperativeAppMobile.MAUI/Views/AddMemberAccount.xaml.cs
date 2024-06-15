using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CooperativeAppMobile.MAUI.Views;

public partial class AddMemberAccount : ContentPage
{
    public ObservableCollection<string> Members { get; set; }
    public ObservableCollection<string> FilteredMembers { get; set; }
    private APIService service { get; set; }

    private async void PopulateAccountType()
    {
        var _account_types = await service.GetAccountTypes();

        foreach (var item in _account_types)
        {
            drp_account_type.Items.Add($"{item.AccountTypeID}:{item.AccountTypeDesc}");
        }
    }
    private async void GetMembers()
    {
        var _members = await service.GetMembers();

        foreach (var member in _members)
        {
            Members.Add($"{member.MemberNumber}:{member.FirstName} {member.LastName}");
        }
    }
    public AddMemberAccount()
    {
        InitializeComponent();
        Members = [];
        FilteredMembers = [];

        BindingContext = this;
        service = new();
    }
    protected override void OnAppearing()
    {
        PopulateAccountType();
    }

    private async void btn_submit_Clicked(object sender, EventArgs e)
    {
        activityInd.IsRunning = true;
        activityInd.IsVisible = true;
        body.IsVisible = false;
        var memberAccount = new MemberAccount
        {
            MemberNumber = txt_member.Text.Split(":").FirstOrDefault(),
            AccountName = txt_account_name.Text,
            AccountNo = txt_account_number.Text,
            BVNNumber = txt_account_bvn.Text,
            AccountStatus = "Active",
            LockStatus = "Unlock",
            DateOpened = Convert.ToDateTime(drp_date_opened.Date),
            InitialDeposit = Convert.ToDecimal(txt_initial_deposit.Text),
            CreatedByStaffID = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            CreatedDate = DateTime.UtcNow,
            UserAccountNo = txt_account_number.Text,
            AccountTypeID = int.Parse(drp_account_type.SelectedItem.ToString().Split(":").FirstOrDefault()),

        };
        await service.ModifyAccounts(memberAccount);
        activityInd.IsRunning = false;
        activityInd.IsVisible = false;
        body.IsVisible = true;
        Toast.Make("record sawved successfully", ToastDuration.Short);
    }

    private void btn_cancel_Clicked(object sender, EventArgs e)
    {

    }

    private void txt_member_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        FilterMembers(searchText);
    }
    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        string selectedItem = e.SelectedItem.ToString();

        // Update TextBox with selected item
        txt_member.Text = selectedItem;

        // Deselect item
        ((ListView)sender).SelectedItem = null;
        dropdownListView.IsVisible = false;
    }
    private void FilterMembers(string searchText)
    {
        GetMembers();

        FilteredMembers.Clear();

        foreach (var item in Members)
        {
            if (item.ToLower().Contains(searchText))
            {
                FilteredMembers.Add(item);
            }
        }
    }
    private void txt_member_Unfocused(object sender, FocusEventArgs e)
    {
        dropdownListView.IsVisible = false;
        FilteredMembers.Clear();
    }

    private void txt_member_Focused(object sender, FocusEventArgs e)
    {
        FilterMembers(txt_member.Text.ToLower());
        dropdownListView.IsVisible = true;
    }
}