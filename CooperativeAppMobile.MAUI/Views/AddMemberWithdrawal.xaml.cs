using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CooperativeAppMobile.MAUI.Views;

public partial class AddMemberWithdrawal : ContentPage, INotifyPropertyChanged
{
    private string _accountName;
    public string AccountName
    {
        get { return _accountName; }
        set
        {
            if (_accountName != value)
            {
                _accountName = value;
                OnPropertyChanged();
            }
        }
    }
    private string _accountNumber;
    public string AccountNumber
    {
        get { return _accountNumber; }
        set
        {
            if (_accountNumber != value)
            {
                _accountNumber = value;
                OnPropertyChanged();
            }
        }
    }
    private decimal _accountBalance;
    public decimal AccountBalance
    {
        get { return _accountBalance; }
        set
        {
            if (_accountBalance != value)
            {
                _accountBalance = value;
                OnPropertyChanged();
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private async void PopulateApproval()
    {
        var statuses = await service.GetStatuses();
        var man_app_tatus = statuses.Where(x => x.WFStatusID == 5).ToList();
        var hod_app_tatus = statuses.Where(x => x.WFStatusID == 2).ToList();
        foreach (var item in man_app_tatus)
        {
            drp_approval_status.Items.Add($"{item.WFStatusID}:{item.Description}");
            drp_approval_status.Items.Insert(0, "Not Approved");
        }
        foreach (var item in hod_app_tatus)
        {
            drp_hod_approval_status.Items.Add($"{item.WFStatusID}:{item.Description}");
            drp_hod_approval_status.Items.Insert(0, "Not Reviewed");
        }
    }
    private async void PopulateWithdrawalType()
    {
        List<string> withdrawal_type = new List<string>()
        {
          "Transfer","Cash"
        };
        foreach (var item in withdrawal_type)
        {
            drp_withdrawal_type.Items.Add(item);
        }
    }
    public ObservableCollection<string> Members { get; set; }
    public ObservableCollection<string> FilteredMembers { get; set; }
    private int memberAccountID { get; set; }
    private APIService service { get; set; }
    private async void PopulateAccountType(int member_id)
    {
        var _account_types = await service.GetAccountTypeByMemberID(member_id);
        foreach (var item in _account_types)
        {
            drp_account_type.Items.Add($"{item.AccountTypeID}:{item.AccountTypeDesc}");
            AccountBalance = await service.GetMemberAccountBalance(txt_member.Text.Split(":").Skip(1).Take(1).FirstOrDefault(), item.AccountTypeID);
        }

    }
    private async void PopulateApprovers()
    {
        var _approvers = await service.GetApprovers();
        foreach (var item in _approvers)
        {
            drp_approved_by_manager.Items.Add($"{item.StaffID}:{item.FirstName} {item.LastName}");
        }
        var _reviewers = await service.GetReviewers();
        foreach (var item in _reviewers)
        {
            drp_approved_by_hod.Items.Add($"{item.StaffID}:{item.FirstName} {item.LastName}");
        }
    }
    private async void PopulateAccountInformation(string member_number)
    {
        var _memberAccount = await service.GetMembersAccount(member_number);
        if (_memberAccount is not null)
        {
            AccountName = _memberAccount.AccountName;
            AccountNumber = Convert.ToString(_memberAccount.AccountNo);
            memberAccountID = _memberAccount.MemberAccountID;
        }
    }
    private async void GetMembers()
    {
        var _members = await service.GetMembers();

        foreach (var member in _members)
        {
            Members.Add($"{member.MemberId}:{member.MemberNumber}:{member.FirstName} {member.LastName}");
        }
    }
    public AddMemberWithdrawal()
    {
        InitializeComponent();
        Members = [];
        FilteredMembers = [];

        BindingContext = this;
        service = new();
    }
    protected override void OnAppearing()
    {
        PopulateApproval();
        PopulateWithdrawalType();
        PopulateApprovers();

    }
    private async void btn_submit_Clicked(object sender, EventArgs e)
    {
        activityInd.IsRunning = true;
        activityInd.IsVisible = true;
        body.IsVisible = false;
        balance_grid.IsVisible = false;
        var memberWithdrawal = new MemberWithdrawal
        {
            MemberNumber = txt_member.Text.Split(":").FirstOrDefault(),
            MemberAccountID = int.Parse(drp_account_type.SelectedItem.ToString().Split(":").FirstOrDefault()),
            WithdrawalDesc = txt_withdrawal_reference.Text,
            WithdrawalAmount = Convert.ToDecimal(txt_amount.Text),
            WithdrawalStatus = "Approved",
            DateApproved = DateTime.UtcNow.Date,
            WithdrawalType = drp_withdrawal_type.SelectedItem.ToString(),
            DebitStatus = "No",
            CreatedBy = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            CreatedByUserRole = "Staff",
            ReferenceUniqueId = Guid.NewGuid().ToString(),
            CreatedDate = DateTime.UtcNow,

        };
        await service.ModifyMembersWithdrawals(memberWithdrawal);
        activityInd.IsRunning = false;
        activityInd.IsVisible = false;
        body.IsVisible = true;
        balance_grid.IsVisible = true;
        Toast.Make("record saved successfully", ToastDuration.Short);
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
        PopulateAccountType(int.Parse(selectedItem.Split(":").FirstOrDefault()));
        // Update TextBox with selected item
        txt_member.Text = selectedItem;
        // Deselect item
        ((ListView)sender).SelectedItem = null;
        dropdownListView.IsVisible = false;
    }
    private async void OnAccountTypeSelected(object sender, EventArgs e)
    {
        if (((Picker)sender).SelectedItem == null)
            return;
        string selectedItem = ((Picker)sender).SelectedItem.ToString();
        PopulateAccountInformation(Convert.ToString(txt_member.Text.Split(":").Skip(1).Take(1).SingleOrDefault()));
        txt_withdrawal_reference.Text = AccountName + " " + selectedItem.Split(":").LastOrDefault() + " " + "withdrawal on " + DateTime.UtcNow.ToLongDateString();
        balance_grid.IsVisible = true;
        lbl_balance.Text = Convert.ToString(AccountBalance);
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