using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace CooperativeAppMobile.MAUI.Views;

public partial class AddMemberSavings : ContentPage, INotifyPropertyChanged
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
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
	public ObservableCollection<string> Members { get; set; }
	public string accountNumber { get; set; }
	public string accountName { get; set; }
	public ObservableCollection<string> FilteredMembers { get; set; }
	private int memberAccountID { get; set; }
	private APIService service { get; set; }
	private async void PopulateAccountType(int member_id)
	{
		var _account_types = await service.GetAccountTypeByMemberID(member_id);

		foreach (var item in _account_types)
		{
			drp_account_type.Items.Add($"{item.AccountTypeID}:{item.AccountTypeDesc}");
		}
	}
	private async void PopulateAccountInformation(string member_number)
	{
		var _memberAccount = await service.GetMembersAccount(member_number);
		if (_memberAccount is not null)
			savings_detail_box.IsVisible = true;
		else
			savings_detail_box.IsVisible = false;
		AccountName = _memberAccount.AccountName;
		AccountNumber = Convert.ToString(_memberAccount.AccountNo);
		memberAccountID = _memberAccount.MemberAccountID;

	}
	private async void GetMembers()
	{
		var _members = await service.GetMembers();
		foreach (var member in _members)
		{
			Members.Add($"{member.MemberId}:{member.MemberNumber}:{member.FirstName} {member.LastName}");
		}
	}
	public AddMemberSavings()
	{
		InitializeComponent();
		Members = [];
		FilteredMembers = [];

		BindingContext = this;
		service = new();
	}
	protected override void OnAppearing()
	{

	}
	private async void btn_submit_Clicked(object sender, EventArgs e)
    {
        activityInd.IsRunning = true;
        activityInd.IsVisible = true;
        body.IsVisible = false;
        MemberSaving_Deposit memberDeposit = new MemberSaving_Deposit();
		memberDeposit.AccountTypeId = int.Parse(drp_account_type.SelectedItem.ToString().Split(":").FirstOrDefault());
		memberDeposit.Amount = Convert.ToDecimal(txt_deposit_amount.Text);
		memberDeposit.ConfirmationStatus = rd_confirmed.IsChecked ? "Confirmed" : rd_pending.IsChecked ? "Pending" : "";
		memberDeposit.MemberNumber = txt_member.Text.ToString().Split(":").Skip(1).Take(1).SingleOrDefault();
		memberDeposit.MemberSavingDefinitionId = null;
		memberDeposit.SavingDate = Convert.ToDateTime(DateTime.UtcNow).Date;
		memberDeposit.SavingDay = Convert.ToDateTime(DateTime.UtcNow).Day;
		memberDeposit.SavingMonth = Convert.ToDateTime(DateTime.UtcNow).Month;
		memberDeposit.SavingYear = Convert.ToDateTime(DateTime.UtcNow).Year;
		memberDeposit.CreatedDate = Convert.ToDateTime(DateTime.UtcNow).Date;
		memberDeposit.AmountDeposited = Convert.ToDecimal(txt_deposit_amount.Text);
		memberDeposit.MemberAccountID = memberAccountID;
		memberDeposit.CreatedBy = int.Parse(await SecureStorage.Default.GetAsync("user_id"));
		memberDeposit.CreatedByUserRole = "Staff";
		memberDeposit.ApprovalStatus = 5;
		memberDeposit.HODDateReviewed = DateTime.UtcNow.AddHours(1).Date;
		memberDeposit.ApprovedBy = int.Parse(await SecureStorage.Default.GetAsync("user_id"));
		memberDeposit.DateApproved = DateTime.UtcNow.AddHours(1).Date;
		memberDeposit.ReferenceUniqueId = Guid.NewGuid().ToString();
		memberDeposit.DepositTime = DateTime.UtcNow.ToShortTimeString();

		await service.ModifyMembersSavings(memberDeposit);

		MemberSavingSummary savingSummary = new MemberSavingSummary();
		savingSummary.MemberNumber = txt_member.Text.ToString().Split(":").Skip(1).Take(1).SingleOrDefault();
		savingSummary.LastSavingAmount = memberDeposit.AmountDeposited;
		savingSummary.LastSavingDate = memberDeposit.SavingDate;
		savingSummary.FirstSavingAmount = memberDeposit.AmountDeposited;
		savingSummary.FirstSavingDate = memberDeposit.SavingDate;
		savingSummary.MemberAccountID = memberDeposit.MemberAccountID;
		savingSummary.AccountStatus = "Active";
		savingSummary.CreatedBy = int.Parse(await SecureStorage.Default.GetAsync("user_id"));
		savingSummary.CreatedDate = Convert.ToDateTime(memberDeposit.CreatedDate).Date;
		savingSummary.AccountBalance = Convert.ToDecimal(memberDeposit.AmountDeposited);
		savingSummary.TotalDepositAmount = Convert.ToDecimal(memberDeposit.AmountDeposited);
		savingSummary.ReferenceUniqueId = Guid.NewGuid().ToString();

		await service.ModifyMembersSavingsSummary(savingSummary);
        activityInd.IsRunning = false;
        activityInd.IsVisible = false;
        body.IsVisible = true;
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
	private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
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