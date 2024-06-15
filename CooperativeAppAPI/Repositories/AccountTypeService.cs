using Azure.Core;

namespace CooperativeAppAPI.Repositories
{
    public interface IAccountTypeService
    {
        public Task<IEnumerable<AccountType>> GetAccountTypeAsync();
        public Task<AccountType> GetAccountTypeAsync(int id);
        public Task<IEnumerable<AccountType>> GetAccountTypeByMember(int MemberID);
    }
    public class AccountTypeService : IAccountTypeService
    {
        private readonly AppDBContext context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        public AccountTypeService(AppDBContext context, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
        }
        public async Task<IEnumerable<AccountType>> GetAccountTypeAsync()
          => await context.AccountType.ToListAsync();
        public async Task<AccountType> GetAccountTypeAsync(int id)
          => await context.AccountType.FindAsync(id);
        public async Task<IEnumerable<AccountType>> GetAccountTypeByMember(int MemberID)
        {
            var acct = context.AccountType
                .Join(context.MemberAccount, i => i.AccountTypeID, ac => ac.AccountTypeID, (i, ac) => new { i, ac })
                .Join(context.Member, tmp => tmp.ac.MemberNumber, ma => ma.MemberNumber, (tmp, ma) => new { tmp.i, tmp.ac, ma })
                .Where(tmp => tmp.ma.MemberId == MemberID && tmp.ac.MemberNumber == tmp.ma.MemberNumber)
                .Select(tmp => new AccountType
                {
                   AccountTypeID = tmp.i.AccountTypeID,
                    AccountTypeDesc =  tmp.i.AccountTypeDesc
                })
                .AsEnumerable()
                .Select(p => new AccountType
                {
                  AccountTypeID =  p.AccountTypeID,
                    AccountTypeDesc = p.AccountTypeDesc
                })
                .ToList();

            return acct;
        }
    }
}
