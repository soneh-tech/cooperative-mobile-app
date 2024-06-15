namespace CooperativeAppAPI.Controllers
{
    [ApiController]
    public class MemberController(IMemberService member) : ControllerBase
    {
        [HttpGet]
        [Route("api/getMembers")]
        public async Task<IActionResult> GetMembers()
        {
            var result = await member.GetMembersAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMember/{id}")]
        public async Task<IActionResult> GetMembers(int id)
        {
            var result = await member.GetMemberAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMembers")]
        public async Task<IActionResult> ModifyMembers([FromBody]Member members)
        {
            var result = await member.ModifyMemberAsync(members);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersAccounts")]
        public async Task<IActionResult> GetMembersAccounts()
        {
            var result = await member.GetMemberAccountsAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersAccounts/{id}")]
        public async Task<IActionResult> GetMembersAccounts(int id)
        {
            var result = await member.GetMemberAccountAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
		[HttpGet]
		[Route("api/getMemberAccount/{member_number}")]
		public async Task<IActionResult> GetMembersAccounts(string member_number)
		{
			var result = await member.GetMemberAccountAsync(member_number);
			return result is not null ? Ok(result) : BadRequest(result);
		}
		[HttpGet]
        [Route("api/getMemberAccountType/{id}")]
        public async Task<IActionResult> GetMemberAccountType(int id)
        {
            var result = await member.GetMemberAccountTypeAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMemberAccounts")]
        public async Task<IActionResult> ModifyAccounts(MemberAccount account)
        {
            var result = await member.ModifyMemberAccountAsync(account);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMemberBanks")]
        public async Task<IActionResult> GetMemberBanks()
        {
            var result = await member.GetMemberBanksAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMemberBank/{id}")]
        public async Task<IActionResult> GetMemberBanks(int id)
        {
            var result = await member.GetMemberBankAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMemberBank")]
        public async Task<IActionResult> ModifyMemberBanks(MemberBank bank)
        {
            var result = await member.ModifyMemberBankAsync(bank);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersSavings")]
        public async Task<IActionResult> GetMembersSavings()
        {
            var result = await member.GetMembersDepositsAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMemberSaving/{id}")]
        public async Task<IActionResult> GetMembersSavings(int id)
        {
            var result = await member.GetMemberDepositAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMemberSavings")]
        public async Task<IActionResult> ModifyMembersSavings(MemberSaving_Deposit deposit)
        {
            var result = await member.ModifyMemberDepositAsync(deposit);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersSavingsSummary")]
        public async Task<IActionResult> GetMembersSavingsSummary()
        {
            var result = await member.GetMemberSavingSummaryAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersSavingsSummary/{membernumber}")]
        public async Task<IActionResult> GetMembersSavingsSummary(string membernumber)
        {
            var result = await member.GetMemberSavingSummaryAsync(membernumber);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMemberSavingsSummary")]
        public async Task<IActionResult> ModifyMembersSavingsSummary(MemberSavingSummary summary)
        {
            var result = await member.ModifyMemberSavingSumaaryAsync(summary);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMembersWithdrawals")]
        public async Task<IActionResult> GetMembersWithdrawals()
        {
            var result = await member.GetMembersWithdrawalsAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getMemberWithdrawal/{id}")]
        public async Task<IActionResult> GetMembersWithdrawals(int id)
        {
            var result = await member.GetMemberWithdrawalAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("api/modifyMemberWithdrawals")]
        public async Task<IActionResult> ModifyMembersWithdrawals(MemberWithdrawal withdrawal)
        {
            var result = await member.ModifyMemberWithdrawalAsync(withdrawal);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }

		[HttpGet]
		[Route("api/getMemberAccountBalance/{memberNumber}/{accountTypeID}")]
		public async Task<IActionResult> GetMemberAccountBalance(string memberNumber,int accountTypeID)
		{
			var result = await member.GetAccountBalance(memberNumber, accountTypeID);
			return result is >= 0 ? Ok(result) : result is <= 0 ? Ok(result) : BadRequest(result);
		}

		[HttpGet]
		[Route("api/getMemberTotalDepositAmount/{memberNumber}/{accountTypeID}")]
		public async Task<IActionResult> GetMemberTotalDepositAmount(string memberNumber, int accountTypeID)
		{
			var result = await member.GetTotalDepositAmount(memberNumber, accountTypeID);
			return result is >= 0 ? Ok(result) : BadRequest(result);
		}
        [HttpGet]
        [Route("api/getApprovers")]
        public async Task<IActionResult> GetApprovers()
        {
            var result = await member.GetApprovers();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getReviewers")]
        public async Task<IActionResult> GetReviewers()
        {
            var result = await member.GetReviewers();
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getStatus")]
        public async Task<IActionResult> GetStatus()
        {
            var result = await member.GetStatus();
            return result is not null ? Ok(result) : BadRequest(result);
        }
    }
}
