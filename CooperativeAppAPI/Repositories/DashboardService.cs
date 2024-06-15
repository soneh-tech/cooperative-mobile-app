namespace CooperativeAppAPI.Repositories
{
    public interface IDashboardService
    {
        public Task<decimal> GetDailyDeposits();
        public Task<decimal> GetDailyWithdrawals();
        public Task<decimal> GetTotalDeposits();
        public Task<decimal> GetTotalWithdrawals();
        public Task<decimal> GetDailyLoans();
        public Task<decimal> GetTotalLoans();
        public Task<int> GetDailyMembers();
        public Task<int> GetTotalMembers();

    }

    public class DashboardService(AppDBContext context) : IDashboardService
    {
        public async Task<int> GetDailyMembers()
            => await context.Member.Where(x => x.CreatedDate == DateTime.UtcNow.Date).CountAsync();
        public async Task<decimal> GetDailyDeposits()
        {
            var result = await context.MemberSaving_Deposit.Where(y => y.SavingDate == DateTime.UtcNow.Date).SumAsync(x => x.Amount);
            return Convert.ToDecimal(result);
        }
        public async Task<decimal> GetDailyWithdrawals()
        {
            var result = await context.MemberWithdrawal.Where(y => y.WithdrawalRequestedDate == DateTime.UtcNow.Date).SumAsync(x => x.WithdrawalAmount);
            return Convert.ToDecimal(result);
        }
        public async Task<decimal> GetDailyLoans()
        {
            var result = await context.LoanApplication.Where(y => y.DateApplied == DateTime.UtcNow.Date).SumAsync(x => x.AmountApproved);
            return Convert.ToDecimal(result);

        }
        public async Task<int> GetTotalMembers()
            => await context.Member.CountAsync();
        public async Task<decimal> GetTotalDeposits()
        {
            var result = await context.MemberSaving_Deposit.SumAsync(x => x.Amount);
            return Convert.ToDecimal(result);

        }
        public async Task<decimal> GetTotalWithdrawals()
        {
            var result = await context.MemberWithdrawal.SumAsync(x => x.WithdrawalAmount);
            return Convert.ToDecimal(result);
        }
        public async Task<decimal> GetTotalLoans()
        {
            var result = await context.LoanApplication.SumAsync(x => x.AmountApproved);
            return Convert.ToDecimal(result);
        }
    }
}
