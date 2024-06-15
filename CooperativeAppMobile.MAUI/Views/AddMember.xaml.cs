using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CooperativeAppMobile.MAUI.Views;

public partial class AddMember : ContentPage
{
    private APIService service { get; set; }

    private async void GenerateMemberNUMAndRandomPass()
    {
        int val = 0;
        var _members = await service.GetMembers();
        if (_members.Count() > 0)
        {

            string MemberNo = string.Empty;
            string Initial = "QC";
            int mbid = _members.Max(u => u.MemberId);
            var us = _members.Where(u => u.MemberId == mbid).ToList().SingleOrDefault();

            if (us != null)
            {
                if (!string.IsNullOrEmpty(us.MemberNumber))
                {
                    var output = Regex.Replace(us.MemberNumber, "[^0-9]+", string.Empty);
                    val = Convert.ToInt32(output) + 1;

                    if (val <= 9)
                    {
                        MemberNo = Initial + "0000000" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 9 && val < 100)
                    {
                        MemberNo = Initial + "000000" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 99 && val < 1000)
                    {
                        MemberNo = Initial + "00000" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 999 && val < 10000)
                    {
                        MemberNo = Initial + "0000" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 9999 && val < 100000)
                    {
                        MemberNo = Initial + "000" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 99999 && val < 1000000)
                    {
                        MemberNo = Initial + "00" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                    else if (val > 999999)
                    {
                        MemberNo = Initial + "0" + Convert.ToString(val);
                        txt_member_num.Text = MemberNo;
                    }
                }
            }
        }
        else
        {
            string MemberNo = "0000000001";
            txt_member_num.Text = MemberNo;
        }

        //string password = GetRandomPassword();
        string password = string.Empty;
        if (val % 2 == 0)
        {
            password = "Me!mB3R" + Convert.ToString(val);
        }
        else
        {
            password = "Me@mB4R" + Convert.ToString(val);
        }
        txt_password.Text = password;
    }
    private async void PopulateAccountType()
    {
        var _account_types = await service.GetAccountTypes();

        foreach (var item in _account_types)
        {
            drp_account_type.Items.Add($"{item.AccountTypeID}:{item.AccountTypeDesc}");
        }
    }
    private async void PopulateStates()
    {
        var _states = await service.GetStates();

        foreach (var item in _states)
        {
            drp_state.Items.Add($"{item.StateId}:{item.StateDescription}");
        }
    }
    private async void PopulateNationality()
    {
        List<string> nationality = new List<string>()
        {
          "Nigerian","Non Nigerian"
        };
        foreach (var item in nationality)
        {
            drp_nationality.Items.Add(item);
        }
    }
    private async void PopulateApproval()
    {
        List<string> approval = new List<string>()
        {
          "Approved","Not Approved"
        };
        foreach (var item in approval)
        {
            drp_mgt_approval.Items.Add(item);
        }
    }

    public AddMember()
    {
        InitializeComponent();
        service = new();
    }
    protected override void OnAppearing()
    {
        GenerateMemberNUMAndRandomPass();
        PopulateAccountType();
        PopulateApproval();
        PopulateNationality();
        PopulateStates();
    }

    private async void btn_submit_Clicked(object sender, EventArgs e)
    {
        activityInd.IsVisible = true;
        activityInd.IsRunning = true;
        body.IsVisible = false;
        var member = new Member
        {
            FirstName = txt_first_name.Text,
            LastName = txt_last_name.Text,
            NINNumber = txt_nin.Text,
            DOB = Convert.ToDateTime(drp_dob.Date),
            Gender = rd_male.IsChecked ? "Male" : rd_female.IsChecked ? "Female" : "",
            MaritalStatus = rd_single.IsChecked ? "Single" : rd_married.IsChecked ? "Married" : "",
            Address = txt_address.Text,
            City = txt_city.Text,
            StateOfOrigin = int.Parse(drp_state.SelectedItem.ToString().Split(":").FirstOrDefault()),
            Nationality = drp_nationality.SelectedItem.ToString(),
            Mobile = txt_phone.Text,
            RoleId = 3,
            RoleDesc = "Member",
            CreatedByUserRole ="Staff",
            Email = txt_email.Text,
            IntroducedBy = txt_introducer.Text,
            NextofKin = txt_nok.Text,
            NextKinAddress = txt_nok_address.Text,
            NextKinPhoneNumber = txt_nok_num.Text,
            AcceptPolicy = rd_acceept.IsChecked ? "Yes" : rd_decline.IsChecked ? "No" : "",
            MemberNumber = txt_member_num.Text,
            Password = txt_password.Text,
            SendSMS = rd_sms_yes.IsChecked ? "Yes" : rd_sms_no.IsChecked ? "No" : "",
            MemberStatus = rd_active.IsChecked ? "Active" : rd_inactive.IsChecked ? "Inactive" : "",
            ManagementApproval = drp_mgt_approval.SelectedItem.ToString(),
            StaffID = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            CreatedBy = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            CreatedDate = DateTime.UtcNow,
            ReferalCode = txt_referal_code.Text,
        };
        var memberAccount = new MemberAccount
        {
            MemberNumber = txt_member_num.Text,
            AccountName = txt_account_name.Text,
            AccountNo = txt_account_number.Text,
            BVNNumber = txt_bvn_number.Text,
            AccountStatus = "Active",
            LockStatus = "Unlock",
            CreatedByStaffID = int.Parse(await SecureStorage.Default.GetAsync("user_id")),
            CreatedDate = DateTime.UtcNow,
            UserAccountNo = txt_account_number.Text,
            AccountTypeID = int.Parse(drp_account_type.SelectedItem.ToString().Split(":").FirstOrDefault()),

        };
        var bankDetails = new MemberBank
        {
            CBName = txt_bank_name.Text,
            CBAccountName = txt_bank_account_name.Text,
            CBAccountNumber = txt_bank_account_number.Text,
            CreatedDate = DateTime.UtcNow,
            MemberNumber = txt_member_num.Text,

        };

        await service.ModifyMembers(member);
        await service.ModifyAccounts(memberAccount);
        await service.ModifyMemberBanks(bankDetails);

        activityInd.IsRunning = false;
        activityInd.IsVisible = false;
        body.IsVisible = true;
        Toast.Make("record sawved successfully", ToastDuration.Short);
    }

    private void btn_cancel_Clicked(object sender, EventArgs e)
    {

    }
}