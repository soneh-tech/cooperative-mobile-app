

namespace CooperativeAppMobile.MAUI.Services
{
    public class APIService
    {
        private HttpClient _httpClient { get; } = new();
        private Member _member { get; set; } = new();
        private StatusResponse _response { get; set; } = new();
        private ObservableCollection<Member> _members { get; set; } = [];
        private MemberAccount _memberAccount { get; set; } = new();
        private ObservableCollection<MemberAccount> _memberAccounts { get; set; } = [];
        private MemberBank _memberBank { get; set; } = new();
        private ObservableCollection<MemberBank> _memberBanks { get; set; } = [];
        private MemberSaving_Deposit _saving { get; set; } = new();
        private ObservableCollection<MemberSaving_Deposit> _savings { get; set; } = [];
        private MemberSavingSummary _savingSummary { get; set; } = new();
        private ObservableCollection<MemberSavingSummary> _savingsSummaries { get; set; } = [];
        private MemberWithdrawal _withdrawal { get; set; } = new();
        private ObservableCollection<MemberWithdrawal> _withdrawals { get; set; } = [];
        private ObservableCollection<AccountType> _accountTypes { get; set; } = [];
        private AccountType _accountType { get; set; } = new();
        private ObservableCollection<tblState> _states { get; set; } = [];
        private Staff _staff { get; set; } = new();
        private ObservableCollection<Staff> _staffs { get; set; } = [];
        private ObservableCollection<WFStatu> _statuses { get; set; } = [];

        public APIService()
        {
            var Token = string.Empty;
            _httpClient.BaseAddress = new Uri("https://175nbsld-7289.uks1.devtunnels.ms/api/");
            // _httpClient.BaseAddress = new Uri("https://shoppingapp.femishsolutions.com/api/");
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", Token);
        }

        //account endpoints
        public async Task<StatusResponse> SignIn(object credentials)
        {
            var result = await _httpClient.PostAsJsonAsync($"login", credentials);

            _response.Status = ((int)result.StatusCode);
            _response = await result.Content.ReadFromJsonAsync<StatusResponse>();

            return _response;
        }


        //dashboard endpoints
        public async Task<decimal> DailyLoan()
        {
            decimal _loan = 0;
            var result = await _httpClient.GetAsync($"getDailyLoans");
            if (result.IsSuccessStatusCode)
            {
                _loan = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _loan;
        }
        public async Task<decimal> DailyDeposit()
        {
            decimal _deposit = 0;
            var result = await _httpClient.GetAsync($"getDailyDeposits");
            if (result.IsSuccessStatusCode)
            {
                _deposit = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _deposit;
        }
        public async Task<int> DailyMembers()
        {
            int _member = 0;
            var result = await _httpClient.GetAsync($"getDailyMembers");
            if (result.IsSuccessStatusCode)
            {
                _member = Convert.ToInt32(await result.Content.ReadAsStringAsync());
            }
            return _member;
        }
        public async Task<decimal> DailyWithdrawals()
        {
            decimal _withdrawals = 0;
            var result = await _httpClient.GetAsync($"getDailyWithdrawals");
            if (result.IsSuccessStatusCode)
            {
                _withdrawals = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _withdrawals;
        }
        public async Task<decimal> TotalLoans()
        {
            decimal _loan = 0;
            var result = await _httpClient.GetAsync($"getTotalLoans");
            if (result.IsSuccessStatusCode)
            {
                _loan = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _loan;
        }
        public async Task<decimal> TotalDeposits()
        {
            decimal _deposit = 0;
            var result = await _httpClient.GetAsync($"getTotalDeposits");
            if (result.IsSuccessStatusCode)
            {
                _deposit = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _deposit;
        }
        public async Task<int> TotalMembers()
        {
            int _member = 0;
            var result = await _httpClient.GetAsync($"getTotalMembers");
            if (result.IsSuccessStatusCode)
            {
                _member = Convert.ToInt32(await result.Content.ReadAsStringAsync());
            }
            return _member;
        }
        public async Task<decimal> TotalWithdrawals()
        {
            decimal _withdrawal = 0;
            var result = await _httpClient.GetAsync($"getTotalWithrawals");
            if (result.IsSuccessStatusCode)
            {
                _withdrawal = Convert.ToDecimal(await result.Content.ReadAsStringAsync());
            }
            return _withdrawal;
        }

        //members endpoints
        public async Task<ObservableCollection<Member>> GetMembers()
        {
            if (_members?.Count > 0)
            {
                return _members;
            }
            var result = await _httpClient.GetAsync($"getMembers");
            if (result.IsSuccessStatusCode)
            {
                _members = await result.Content.ReadFromJsonAsync<ObservableCollection<Member>>();
            }
            return _members;
        }
        public async Task<Member> GetMembers(int member_id)
        {
            var result = await _httpClient.GetAsync($"getMember/{member_id}");
            if (result.IsSuccessStatusCode)
            {
                _member = await result.Content.ReadFromJsonAsync<Member>();
            }
            return _member;
        }
        public async Task<bool> ModifyMembers(Member members)
        {
            if (members is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMembers", members);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        //member account endpoints
        public async Task<ObservableCollection<MemberAccount>> GetMembersAccounts()
        {
            if (_memberAccounts?.Count > 0)
            {
                return _memberAccounts;
            }
            var result = await _httpClient.GetAsync($"getMembersAccounts");
            if (result.IsSuccessStatusCode)
            {
                _memberAccounts = await result.Content.ReadFromJsonAsync<ObservableCollection<MemberAccount>>();
            }
            return _memberAccounts;
        }
		public async Task<MemberAccount> GetMembersAccounts(int id)
		{
			var result = await _httpClient.GetAsync($"getMembersAccounts/{id}");
			if (result.IsSuccessStatusCode)
			{
				_memberAccount = await result.Content.ReadFromJsonAsync<MemberAccount>();
			}
			return _memberAccount;
		}
		public async Task<MemberAccount> GetMembersAccount(string member_number)
		{
			var result = await _httpClient.GetAsync($"getMemberAccount/{member_number}");
			if (result.IsSuccessStatusCode)
			{
				_memberAccount = await result.Content.ReadFromJsonAsync<MemberAccount>();
			}
			return _memberAccount;
		}
		public async Task<MemberAccount> GetMemberAccountType(int id)
        {
            var result = await _httpClient.GetAsync($"getMemberAccountType/{id}");
            if (result.IsSuccessStatusCode)
            {
                _memberAccount = await result.Content.ReadFromJsonAsync<MemberAccount>();
            }
            return _memberAccount;
        }
        public async Task<bool> ModifyAccounts(MemberAccount account)
        {
            if (account is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMemberAccounts", account);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        //member deposit endpoints
        public async Task<ObservableCollection<MemberSaving_Deposit>> GetMembersSavings()
        {
            if (_savings?.Count > 0)
            {
                return _savings;
            }
            var result = await _httpClient.GetAsync($"getMembersSavings");
            if (result.IsSuccessStatusCode)
            {
                _savings = await result.Content.ReadFromJsonAsync<ObservableCollection<MemberSaving_Deposit>>();
            }
            return _savings;
        }
        public async Task<MemberSaving_Deposit> GetMembersSavings(int id)
        {
            var result = await _httpClient.GetAsync($"getMemberSaving/{id}");
            if (result.IsSuccessStatusCode)
            {
                _saving = await result.Content.ReadFromJsonAsync<MemberSaving_Deposit>();
            }
            return _saving;
        }
        public async Task<bool> ModifyMembersSavings(MemberSaving_Deposit deposit)
        {
            if (deposit is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMemberSavings", deposit);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        //member withdrawal endpoints
        public async Task<ObservableCollection<MemberWithdrawal>> GetMembersWithdrawals()
        {
            if (_withdrawals?.Count > 0)
            {
                return _withdrawals;
            }
            var result = await _httpClient.GetAsync($"getMembersWithdrawals");
            if (result.IsSuccessStatusCode)
            {
                _withdrawals = await result.Content.ReadFromJsonAsync<ObservableCollection<MemberWithdrawal>>();
            }
            return _withdrawals;
        }
        public async Task<MemberWithdrawal> GetMembersWithdrawals(int id)
        {
            var result = await _httpClient.GetAsync($"getMemberWithdrawal/{id}");
            if (result.IsSuccessStatusCode)
            {
                _withdrawal = await result.Content.ReadFromJsonAsync<MemberWithdrawal>();
            }
            return _withdrawal;
        }
        public async Task<bool> ModifyMembersWithdrawals(MemberWithdrawal withdrawal)
        {
            if (withdrawal is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMemberWithdrawals", withdrawal);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        //member bank endpoints
        public async Task<ObservableCollection<MemberBank>> GetMemberBanks()
        {
            if (_savings?.Count > 0)
            {
                return _memberBanks;
            }
            var result = await _httpClient.GetAsync($"getMemberBank");
            if (result.IsSuccessStatusCode)
            {
                _memberBanks = await result.Content.ReadFromJsonAsync<ObservableCollection<MemberBank>>();
            }
            return _memberBanks;
        }
        public async Task<MemberBank> GetMemberBanks(int id)
        {
            var result = await _httpClient.GetAsync($"getMemberBank/{id}");
            if (result.IsSuccessStatusCode)
            {
                _memberBank = await result.Content.ReadFromJsonAsync<MemberBank>();
            }
            return _memberBank;
        }
        public async Task<bool> ModifyMemberBanks(MemberBank memberBank)
        {
            if (memberBank is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMemberBank", memberBank);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

		public async Task<decimal> GetMemberAccountBalance(string memberNumber,int accountTypeId)
		{
			if (memberNumber is null || accountTypeId is not > 0)
				return 0;
			var result = await _httpClient.GetAsync($"getMemberAccountBalance/{memberNumber}/{accountTypeId}");
			if (result.IsSuccessStatusCode)
			{
				return Convert.ToDecimal(result.Content.ReadAsStringAsync().Result);
			}
			return 0;
		}
		public async Task<decimal> GetMemberTotalDepositAmount(string memberNumber, int accountTypeId)
		{
			if (memberNumber is null || accountTypeId is not > 0)
				return 0;
			var result = await _httpClient.GetAsync($"getMemberTotalDepositAmount/{memberNumber}/{accountTypeId}");
			if (result.IsSuccessStatusCode)
			{
				return Convert.ToDecimal(result.Content.ReadAsStringAsync().Result);
			}
			return 0;
		}

		//account types endpoints
		public async Task<ObservableCollection<AccountType>> GetAccountTypes()
        {
            var result = await _httpClient.GetAsync($"getAccountTypes");
            if (result.IsSuccessStatusCode)
            {
                _accountTypes = await result.Content.ReadFromJsonAsync<ObservableCollection<AccountType>>();
            }
            return _accountTypes;
        }
        public async Task<AccountType> GetAccountType(int? id)
        {
            var result = await _httpClient.GetAsync($"getAccountType/{id}");
            if (result.IsSuccessStatusCode)
            {
                _accountType = await result.Content.ReadFromJsonAsync<AccountType>();
            }
            return _accountType;
        }
        public async Task<int> GetAccountTypeID(string membernumber)
        {
            var id = 0;
            var result = await _httpClient.GetAsync($"getAccountTypeID/{membernumber}");
            if (result.IsSuccessStatusCode)
            {
                id = int.Parse(await result.Content.ReadAsStringAsync());
            }
            return id;
        }
        public async Task<ObservableCollection<AccountType>> GetAccountTypeByMemberID(int member_id)
        {
            if (_accountTypes?.Count > 0)
            {
                return _accountTypes;
            }
            var result = await _httpClient.GetAsync($"getAccountTypeByMember/{member_id}");
            if (result.IsSuccessStatusCode)
            {
                _accountTypes = await result.Content.ReadFromJsonAsync<ObservableCollection<AccountType>>();
            }
            return _accountTypes;
        }

        //states endpoints
        public async Task<ObservableCollection<tblState>> GetStates()
        {
            var result = await _httpClient.GetAsync($"getStates");
            if (result.IsSuccessStatusCode)
            {
                _states = await result.Content.ReadFromJsonAsync<ObservableCollection<tblState>>();
            }
            return _states;
        }

        //member summary endpoints
        public async Task<bool> ModifyMembersSavingsSummary(MemberSavingSummary savingSummary)
        {
            if (savingSummary is null)
                return false;
            var result = await _httpClient.PostAsJsonAsync("modifyMemberSavingsSummary", savingSummary);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Staff>> GetApprovers()
        {
            var result = await _httpClient.GetAsync($"getApprovers");
            if (result.IsSuccessStatusCode)
            {
                _staffs = await result.Content.ReadFromJsonAsync<ObservableCollection<Staff>>();
            }
            return _staffs;
        }
        public async Task<IEnumerable<Staff>> GetReviewers()
        {
            var result = await _httpClient.GetAsync($"getReviewers");
            if (result.IsSuccessStatusCode)
            {
                _staffs = await result.Content.ReadFromJsonAsync<ObservableCollection<Staff>>();
            }
            return _staffs;
        }
        public async Task<IEnumerable<WFStatu>> GetStatuses()
        {
            var result = await _httpClient.GetAsync($"getStatus");
            if (result.IsSuccessStatusCode)
            {
                _statuses = await result.Content.ReadFromJsonAsync<ObservableCollection<WFStatu>>();
            }
            return _statuses;
        }
    }

}
