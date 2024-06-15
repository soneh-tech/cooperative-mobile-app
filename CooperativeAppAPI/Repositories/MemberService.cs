using System.Security.AccessControl;

namespace CooperativeAppAPI.Repositories
{
    public interface IMemberService
    {
        public Task<IEnumerable<Member>> GetMembersAsync();
        public Task<Member> GetMemberAsync(int MemberID);
        public Task<StatusResponse> ModifyMemberAsync(Member member);
        public Task<IEnumerable<MemberBank>> GetMemberBanksAsync();
        public Task<MemberBank> GetMemberBankAsync(int MemberBankID);
        public Task<StatusResponse> ModifyMemberBankAsync(MemberBank memberBank);
        public Task<IEnumerable<MemberAccount>> GetMemberAccountsAsync();
        public Task<MemberAccount> GetMemberAccountAsync(int AccountID);
        public Task<MemberAccount> GetMemberAccountAsync(string member_number);
        public Task<MemberAccount> GetMemberAccountTypeAsync(int AccountID);
        public Task<StatusResponse> ModifyMemberAccountAsync(MemberAccount account);
        public Task<IEnumerable<MemberSaving_Deposit>> GetMembersDepositsAsync();
        public Task<MemberSaving_Deposit> GetMemberDepositAsync(int DepositID);
        public Task<StatusResponse> ModifyMemberDepositAsync(MemberSaving_Deposit deposit);
        public Task<IEnumerable<MemberWithdrawal>> GetMembersWithdrawalsAsync();
        public Task<MemberWithdrawal> GetMemberWithdrawalAsync(int WithdrawalID);
        public Task<StatusResponse> ModifyMemberWithdrawalAsync(MemberWithdrawal withdrawal);
        public Task<IEnumerable<MemberSavingSummary>> GetMemberSavingSummaryAsync();
        public Task<MemberSavingSummary> GetMemberSavingSummaryAsync(string memberNumber);
        public Task<StatusResponse> ModifyMemberSavingSumaaryAsync(MemberSavingSummary summary);
        public Task<decimal> GetTotalDepositAmount(string MemberNumber, int AcctTypeId);
        public Task<decimal> GetAccountBalance(string MemberNumber, int AcctTypeId);
        public Task<IEnumerable<object>> GetApprovers();
        public Task<IEnumerable<object>> GetReviewers();
        Task<IEnumerable<WFStatu>> GetStatus();
    }

    public class MemberService(AppDBContext context) : IMemberService
    {
        public async Task<MemberAccount> GetMemberAccountAsync(int AccountID)
         => await context.MemberAccount.FindAsync(AccountID);
        public async Task<MemberAccount> GetMemberAccountAsync(string member_number)
        => await context.MemberAccount.Where(x => x.MemberNumber == member_number).SingleOrDefaultAsync();
        public async Task<IEnumerable<MemberAccount>> GetMemberAccountsAsync()
         => await context.MemberAccount.ToListAsync();

        public async Task<MemberAccount> GetMemberAccountTypeAsync(int AccountID)
        {
            return await context.MemberAccount.Where(x => x.AccountTypeID == AccountID).SingleOrDefaultAsync();
        }

        public async Task<Member> GetMemberAsync(int MemberID)
         => await context.Member.FindAsync(MemberID);

        public async Task<MemberBank> GetMemberBankAsync(int MemberBankID)
         => await context.MemberBank.FindAsync(MemberBankID);

        public async Task<IEnumerable<MemberBank>> GetMemberBanksAsync()
         => await context.MemberBank.ToListAsync();

        public async Task<MemberSaving_Deposit> GetMemberDepositAsync(int DepositID)
         => await context.MemberSaving_Deposit.FindAsync(DepositID);

        public async Task<IEnumerable<Member>> GetMembersAsync()
         => await context.Member.ToListAsync();

        public async Task<IEnumerable<MemberSavingSummary>> GetMemberSavingSummaryAsync()
         => await context.MemberSavingSummary.ToListAsync();

        public async Task<MemberSavingSummary> GetMemberSavingSummaryAsync(string memberNumber)
         => await context.MemberSavingSummary.Where(x => x.MemberNumber == memberNumber).SingleOrDefaultAsync();

        public async Task<IEnumerable<MemberSaving_Deposit>> GetMembersDepositsAsync()
         => await context.MemberSaving_Deposit.ToListAsync();

        public async Task<IEnumerable<MemberWithdrawal>> GetMembersWithdrawalsAsync()
         => await context.MemberWithdrawal.ToListAsync();

        public async Task<MemberWithdrawal> GetMemberWithdrawalAsync(int WithdrawalID)
         => await context.MemberWithdrawal.FindAsync(WithdrawalID);

        public async Task<StatusResponse> ModifyMemberAccountAsync(MemberAccount account)
        {
            var response = new StatusResponse();
            try
            {
                if (account.MemberAccountID is 0)
                {
                    await context.MemberAccount.AddAsync(account);
                    response.Status = 200;
                    response.Result = "Member Account successfully created";
                }
                else
                {
                    context.MemberAccount.Update(account);
                    response.Status = 200;
                    response.Result = "Member Account record successfully updated";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;

        }

        public async Task<StatusResponse> ModifyMemberAsync(Member member)
        {
            var response = new StatusResponse();
            try
            {

                if (!(await context.Member.AnyAsync(x => x.MemberNumber == member.MemberNumber)))
                {

                    var userencryptedpass = HshClass.ComputeHash(member.Password.Trim(), "SHA512", null);
                    var UserId = Guid.NewGuid().ToString();
                    member.Password = userencryptedpass;
                    member.UserId = UserId;

                    await context.Member.AddAsync(member);
                    response.Status = 200;
                    response.Result = "Member successfully created";
                }
                else
                {
                    context.Member.Update(member);
                    response.Status = 200;
                    response.Result = "Member record successfully updated";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }

        public async Task<StatusResponse> ModifyMemberBankAsync(MemberBank memberBank)
        {
            var response = new StatusResponse();
            try
            {

                if (!(await context.MemberBank.AnyAsync(x => x.MemberNumber == memberBank.MemberNumber)))
                {

                    await context.MemberBank.AddAsync(memberBank);
                    response.Status = 200;
                    response.Result = "Member Bank successfully created";
                }
                else
                {
                    context.MemberBank.Update(memberBank);
                    response.Status = 200;
                    response.Result = "Member record successfully updated";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }

        public async Task<StatusResponse> ModifyMemberDepositAsync(MemberSaving_Deposit deposit)
        {
            var response = new StatusResponse();
            try
            {
                if (deposit.MemberSavingId is 0)
                {
                    await context.MemberSaving_Deposit.AddAsync(deposit);
                    response.Status = 200;
                    response.Result = "Member Deposit successfully recorded";
                }
                else
                {
                    context.MemberSaving_Deposit.Update(deposit);
                    response.Status = 200;
                    response.Result = "Member Deposit record successfully updated";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }

        public async Task<StatusResponse> ModifyMemberSavingSumaaryAsync(MemberSavingSummary summary)
        {
            var response = new StatusResponse();
            try
            {
                var saving_summary = await context.MemberSavingSummary.FirstOrDefaultAsync(x => x.MemberAccountID == summary.MemberAccountID);
                if (saving_summary != null)
                {
                    saving_summary.LastSavingDate = DateTime.UtcNow.Date;
                    saving_summary.LastSavingAmount = summary.LastSavingAmount;
                    saving_summary.AccountBalance += summary.AccountBalance;
                    saving_summary.TotalDepositAmount += summary.AccountBalance;
                    context.Entry(saving_summary).State = EntityState.Modified;
                    response.Status = 200;
                    response.Result = "Member deposit summary successfully updated";
                }
                else
                {
                    await context.MemberSavingSummary.AddAsync(summary);
                    response.Status = 200;
                    response.Result = "Member deposit summary successfully recorded";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }

        public async Task<StatusResponse> ModifyMemberWithdrawalAsync(MemberWithdrawal withdrawal)
        {
            var response = new StatusResponse();
            try
            {
                var member = await context.Member.SingleOrDefaultAsync(x => x.MemberNumber == withdrawal.MemberNumber);
                var savingsAcct = await context.MemberSavingSummary
                                .Join(context.MemberAccount, k => k.MemberAccountID, acc => acc.MemberAccountID, (k, acc) => new { k, acc })
                                .Where(x => x.acc.MemberNumber == x.k.MemberNumber &&
                                            x.k.MemberAccountID == withdrawal.MemberAccountID &&
                                            x.k.MemberAccountID == withdrawal.MemberAccountID &&
                                            x.k.MemberNumber == withdrawal.MemberNumber)
                                .Select(x => x.k)
                                .SingleOrDefaultAsync();

                var accttype = await context.AccountType
                    .Join(context.MemberAccount, da => da.AccountTypeID, ma => ma.AccountTypeID, (da, ma) => new { da, ma })
                    .Join(context.Member, x => x.ma.MemberNumber, me => me.MemberNumber, (x, me) => new { x.da, x.ma, me })
                    .Where(x => x.da.AccountTypeID == x.ma.AccountTypeID && x.me.MemberId == member.MemberId && x.ma.MemberAccountID == withdrawal.MemberAccountID)
                    .Select(x => x.da)
                    .SingleOrDefaultAsync();


                decimal Minimumbal = 0, withdrawalamt = 0, total = 0;
                Minimumbal = Convert.ToDecimal(accttype.MinimumBalance);
                withdrawalamt = Convert.ToDecimal(withdrawal.WithdrawalAmount);
                total = Minimumbal + withdrawalamt;
                decimal TotalDeductionAmt = 0, AccountBalance = 0;

                if (savingsAcct.AccountBalance > 0 && savingsAcct.AccountBalance > total)
                {
                    await context.MemberWithdrawal.AddAsync(withdrawal);
                    response.Status = 200;
                    response.Result = "Member Withdrawal successfully recorded";
                }
                else
                {
                    context.MemberWithdrawal.Update(withdrawal);
                    response.Status = 200;
                    response.Result = "Member Withdrawal record successfully updated";
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }
        public async Task<decimal> GetAccountBalance(string MemberNumber, int AcctTypeId)
        {
            var AccountBalance = await context.MemberSavingSummary
                .Join(context.MemberAccount, i => i.MemberAccountID, m => m.MemberAccountID, (i, m) => new { i, m })
                .Where(x => x.i.MemberNumber == MemberNumber && x.m.AccountTypeID == AcctTypeId)
                .Select(x => x.i.AccountBalance)
                .SumAsync();

            return Convert.ToDecimal(AccountBalance);

        }
        public async Task<decimal> GetTotalDepositAmount(string MemberNumber, int AcctTypeId)
        {
            var DepositAmount = await context.MemberSavingSummary
               .Join(context.MemberAccount, i => i.MemberAccountID, m => m.MemberAccountID, (i, m) => new { i, m })
               .Where(x => x.i.MemberNumber == MemberNumber && x.m.AccountTypeID == AcctTypeId)
               .Select(x => x.i.TotalDepositAmount)
               .SumAsync();

            return Convert.ToDecimal(DepositAmount);

        }

        public async Task<IEnumerable<object>> GetApprovers()
        {
            var approvers = await context.Staff
                .Where(i => i.StaffStatus == "Active" && i.RoleId == 1 &&
                            i.StaffNumber != "fma" && i.StaffNumber != "SAdmin" && i.StaffNumber != "SuperAdmin")
                .Select(i => new
                {
                    i.StaffID,
                    i.FirstName,
                    i.LastName
                })
                .ToListAsync();

            return approvers;

        }
        public async Task<IEnumerable<object>> GetReviewers()
        {
            var reviewers = await context.Staff
                  .Where(i => i.StaffStatus == "Active" && i.RoleId == 4)
                  .Select(i => new
                  {
                      i.StaffID,
                      i.FirstName,
                      i.LastName
                  })
                  .ToListAsync();

            return reviewers;

        }

        public async Task<IEnumerable<WFStatu>> GetStatus()
          => await context.WFStatus.ToListAsync();
    }

    //public class MemberService : IMemberService
    //{
    //    private readonly AppDBContext context;
    //    private readonly IConfiguration configuration;
    //    private readonly IWebHostEnvironment hostingEnvironment;


    //   // HttpCookie user = HttpContext.Current.Request.Cookies["UserInfo"];
    //    private string UserName, sname, RoleDesc, EmailAddress;
    //    private int RoleId, StaffID;
    //    public const string CodeKey = "Code";
    //    public const string UserIdKey = "UserId";
    //    public string UserId = string.Empty;
    //    public string UserAccounNo = string.Empty, Previous_PageURL = "";
    //    private UserAuditClass uac { get; set; }

    //    public MemberService(AppDBContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    //    {
    //        this.context = context;
    //        this.configuration = configuration;
    //        this.hostingEnvironment = hostingEnvironment;
    //    }
    //    /*
    //    private async Task<string> ProcessedPhoto(Member member)
    //    {
    //        string ImageURL = string.Empty;
    //        if (member.Photo != null)
    //        {
    //            string uniqueFile;
    //            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
    //            uniqueFile = Guid.NewGuid().ToString() + "_" + member.Photo.FileName;
    //            string filePath = Path.Combine(uploadsFolder, uniqueFile);
    //            using (var filestream = new FileStream(filePath, FileMode.Create))
    //            { await member.Photo.CopyToAsync(filestream); }

    //            ImageURL = $"{configuration["Host:appUrl"]}/images/{uniqueFile}";

    //        }
    //        return ImageURL;
    //    }
    //    private async Task<string> ProcessedSignaturePhoto(Member member)
    //    {
    //        string ImageURL = string.Empty;
    //        if (member.Photo != null)
    //        {
    //            string uniqueFile;
    //            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
    //            uniqueFile = Guid.NewGuid().ToString() + "_" + member.SignaturePhoto.FileName;
    //            string filePath = Path.Combine(uploadsFolder, uniqueFile);
    //            using (var filestream = new FileStream(filePath, FileMode.Create))
    //            { await member.SignaturePhoto.CopyToAsync(filestream); }

    //            ImageURL = $"{configuration["Host:appUrl"]}/images/{uniqueFile}";

    //        }
    //        return ImageURL;
    //    }
    //    */

    //    protected void ExecProc(int? tableId, string tname, string op, string dt)
    //    {
    //        using (SqlConnection con = new SqlConnection())
    //        {
    //            con.ConnectionString = configuration.GetConnectionString("SqlConnection");
    //            using (SqlCommand cmd = new SqlCommand())
    //            {
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.CommandText = "AuditTrailProc";
    //                cmd.Parameters.AddWithValue("@tabId", tableId);
    //                cmd.Parameters.AddWithValue("@tabName", tname);
    //                cmd.Parameters.AddWithValue("@opr", op);
    //                cmd.Parameters.AddWithValue("@date", dt);
    //                cmd.Connection = con;
    //                con.Open();
    //                cmd.ExecuteNonQuery();
    //                con.Close();
    //            }
    //        }
    //    }

    //    public string GetLast(string source, int length)
    //    {
    //        if (length >= source.Length)
    //            return source;
    //        return source.Substring(source.Length - length);
    //    }
    //    public string GetReceiptNo()
    //    {
    //        string num = string.Empty;

    //        var pa = (from k in context.tblReceipt
    //                  where (k.ReceiptNumber != null && k.ReceiptNumber != "")
    //                  orderby k.ReceiptID descending
    //                  select k.ReceiptID).ToList().Max();
    //        if (pa > 0)
    //        {
    //            int stid = Convert.ToInt32(pa);
    //            int id = 1;

    //            var us = (from u in context.tblReceipt
    //                      where u.ReceiptID == stid
    //                      select u).ToList().SingleOrDefault();
    //            if (us != null)
    //            {
    //                if (us.ReceiptNumber.Contains("QT"))
    //                {
    //                    string SubStringmid = GetLast(us.ReceiptNumber, 5);
    //                    int submid = (Convert.ToInt32(SubStringmid) + id);

    //                    if (Convert.ToInt32(submid) <= 9)
    //                    {
    //                        num = "QT0000" + submid;
    //                    }
    //                    else if (Convert.ToInt32(submid) <= 99)
    //                    {
    //                        num = "QT000" + submid;
    //                    }
    //                    else if (Convert.ToInt32(submid) <= 999)
    //                    {
    //                        num = "QT00" + submid;
    //                    }
    //                    else if (Convert.ToInt32(submid) <= 9999)
    //                    {
    //                        num = "QT0" + submid;
    //                    }
    //                    else if (Convert.ToInt32(submid) <= 99999)
    //                    {
    //                        num = "QT" + submid;
    //                    }
    //                }
    //                else
    //                {
    //                    num = "QT00001";
    //                }
    //            }
    //            else
    //            {
    //                num = "QT00001";
    //            }
    //        }
    //        return num;
    //    }

    //    public async Task<StatusResponse> AddMemberAccountAsync(Member member, string ops, string account_type, string payment_type, MemberBank memberBank, MemberAccount memberAccount)
    //    {
    //        var response = new StatusResponse();
    //        try
    //        {

    //            if (ops == "Submit")
    //            {
    //                string MemberNumber = string.Empty;

    //                if (!string.IsNullOrEmpty(member.MemberId.ToString()))
    //                {
    //                    string MemberName = member.FirstName + " " + member.LastName;
    //                    int MemberId = member.MemberId;

    //                    DateTime edate;
    //                    edate = Convert.ToDateTime(DateTime.Now.Date).Date;

    //                    var mem = (from i in context.Member
    //                               where i.MemberId == MemberId
    //                               select i).ToList().SingleOrDefault();

    //                    MemberAccount m = new MemberAccount();
    //                    if (!string.IsNullOrEmpty(memberAccount.MemberNumber))
    //                        m.AccountNo = memberAccount.AccountNo;
    //                    if (UserAccounNo != "")
    //                        m.UserAccountNo = memberAccount.UserAccountNo;
    //                    else
    //                        m.UserAccountNo = memberAccount.UserAccountNo;
    //                    m.AccountStatus = "Active";
    //                    m.LockStatus = "Unlock";

    //                    if (mem != null)
    //                        m.MemberNumber = Convert.ToString(mem.MemberNumber);
    //                    if (!string.IsNullOrEmpty(memberAccount.AccountName))
    //                        m.AccountName = Convert.ToString(memberAccount.AccountName);
    //                    m.AccountTypeID = Convert.ToInt32(memberAccount.AccountTypeID);
    //                    m.DateOpened = Convert.ToDateTime(memberAccount.DateOpened).Date;
    //                    if (!string.IsNullOrEmpty(memberAccount.InitialDeposit.ToString()))
    //                        m.InitialDeposit = Convert.ToDecimal(memberAccount.InitialDeposit);
    //                    m.CreatedByStaffID = StaffID;
    //                    m.CreatedDate = DateTime.Now.Date;
    //                    if (!string.IsNullOrEmpty(memberAccount.BVNNumber))
    //                        m.BVNNumber = memberAccount.BVNNumber;

    //                    //Generate unique code for every user
    //                    Guid obj = Guid.NewGuid();
    //                    UserId = Convert.ToString(obj);
    //                    m.ReferenceUniqueId = UserId;//Unique Identifier

    //                    //Check if Record Already Exist.
    //                    var chk = (from i in context.MemberAccount
    //                               where i.AccountNo == m.AccountNo
    //                               select i).ToList().SingleOrDefault();
    //                    if (chk == null)
    //                    {
    //                        await context.MemberAccount.AddAsync(m);
    //                        await context.SaveChangesAsync();

    //                        if (m.MemberAccountID > 0)
    //                        {
    //                            if (!string.IsNullOrEmpty(memberAccount.InitialDeposit.ToString()))
    //                            {
    //                                decimal InitialDeposit = Convert.ToDecimal(memberAccount.InitialDeposit.ToString());

    //                                int AccountTypeId = Convert.ToInt32(memberAccount.AccountTypeID);
    //                                var act = (from n in context.MemberAccount where n.AccountTypeID == AccountTypeId && n.MemberNumber == m.MemberNumber select n).ToList().SingleOrDefault();

    //                                MemberSaving_Deposit md = new MemberSaving_Deposit();
    //                                md.AccountTypeId = AccountTypeId;
    //                                md.Amount = InitialDeposit;
    //                                md.ConfirmationStatus = "Confirmed";
    //                                md.ConfirmDate = edate;
    //                                md.MemberNumber = m.MemberNumber;
    //                                md.SavingDate = Convert.ToDateTime(edate).Date;
    //                                md.SavingDay = Convert.ToDateTime(edate).Day;
    //                                md.SavingMonth = Convert.ToDateTime(edate).Month;
    //                                md.SavingYear = Convert.ToDateTime(edate).Year;
    //                                md.CreatedDate = Convert.ToDateTime(edate).Date;
    //                                md.AmountDeposited = InitialDeposit;
    //                                md.MemberAccountID = m.MemberAccountID;

    //                                //Generate unique code for every user
    //                                Guid obja = Guid.NewGuid();
    //                                UserId = Convert.ToString(obja);
    //                                md.ReferenceUniqueId = UserId;//Unique Identifier

    //                                var dep = (from k in context.MemberSaving_Deposit
    //                                           where k.SavingDate == edate && k.MemberNumber == act.MemberNumber
    //                                           && k.MemberAccountID == act.MemberAccountID && k.AmountDeposited == InitialDeposit
    //                                           && k.AccountTypeId == m.AccountTypeID
    //                                           select k).ToList().SingleOrDefault();
    //                                if (dep == null)
    //                                {
    //                                    await context.MemberSaving_Deposit.AddAsync(md);
    //                                    await context.SaveChangesAsync();

    //                                    //Insert into MemberSavingsSummary at this point
    //                                    if (md.MemberSavingId > 0)
    //                                    {
    //                                        MemberSavingSummary pm = new MemberSavingSummary();

    //                                        pm.MemberNumber = md.MemberNumber;
    //                                        pm.LastSavingAmount = md.AmountDeposited;
    //                                        pm.LastSavingDate = md.SavingDate;
    //                                        pm.FirstSavingAmount = md.AmountDeposited;
    //                                        pm.FirstSavingDate = md.SavingDate;
    //                                        pm.MemberAccountID = md.MemberAccountID;
    //                                        pm.AccountStatus = "Active";
    //                                        pm.CreatedBy = StaffID;
    //                                        pm.CreatedDate = Convert.ToDateTime(md.CreatedDate).Date;
    //                                        pm.AccountBalance = Convert.ToDecimal(getAccountBalance(pm.MemberNumber, AccountTypeId)) + md.AmountDeposited;
    //                                        pm.TotalDepositAmount = Convert.ToDecimal(getTotalDepositAmount(pm.MemberNumber, AccountTypeId) + md.AmountDeposited);

    //                                        //Generate unique code for every user
    //                                        Guid objj = Guid.NewGuid();
    //                                        UserId = Convert.ToString(objj);
    //                                        pm.ReferenceUniqueId = UserId;//Unique Identifier

    //                                        var pss = (from x in context.MemberSavingSummary
    //                                                   join am in context.MemberAccount on x.MemberAccountID equals am.MemberAccountID
    //                                                   where am.AccountTypeID == md.AccountTypeId && x.MemberNumber == md.MemberNumber
    //                                                   && x.MemberAccountID == md.MemberAccountID
    //                                                   select x).ToList().SingleOrDefault();
    //                                        if (pss == null)
    //                                        {

    //                                            await context.MemberSavingSummary.AddAsync(pm);
    //                                            await context.SaveChangesAsync();
    //                                        }
    //                                        else
    //                                        {
    //                                            pss.LastSavingAmount = md.AmountDeposited;
    //                                            pss.LastSavingDate = md.SavingDate;
    //                                            pss.AccountStatus = "Active";
    //                                            pss.AccountBalance = Convert.ToDecimal(getAccountBalance(act.MemberNumber, AccountTypeId)) + md.AmountDeposited;
    //                                            pss.TotalDepositAmount = Convert.ToDecimal(getTotalDepositAmount(act.MemberNumber, AccountTypeId) + md.AmountDeposited);

    //                                            await context.SaveChangesAsync();
    //                                        }

    //                                        var transbal = (from x in context.MemberTransactionBalance
    //                                                        join am in context.MemberAccount on x.MemberAccountID equals am.MemberAccountID
    //                                                        where am.AccountTypeID == md.AccountTypeId && x.MemberNumber == md.MemberNumber
    //                                                        && x.MemberAccountID == md.MemberAccountID && x.TransactionDate == edate
    //                                                        && x.TransactionId == md.MemberSavingId && x.TransactionType == "Credit"
    //                                                        select x).ToList().SingleOrDefault();
    //                                        if (transbal == null)
    //                                        {
    //                                            MemberTransactionBalance tb = new MemberTransactionBalance();

    //                                            //Generate unique code for every user
    //                                            Guid objja = Guid.NewGuid();
    //                                            UserId = Convert.ToString(objja);
    //                                            tb.ReferenceUniqueId = UserId;//Unique Identifier
    //                                            tb.MemberNumber = md.MemberNumber;
    //                                            tb.Credit = md.AmountDeposited;
    //                                            tb.TransactionDate = md.ConfirmDate;
    //                                            tb.MemberAccountID = md.MemberAccountID;
    //                                            tb.TransactionId = md.MemberSavingId;
    //                                            tb.TransactionType = "Credit";
    //                                            tb.Description = "Member Deposit Payment";
    //                                            tb.CreatedBy = StaffID;
    //                                            tb.CreatedDate = Convert.ToDateTime(md.CreatedDate).Date;
    //                                            //update Daily transaction balance
    //                                            tb.Balance = Convert.ToDecimal(getAccountBalance(act.MemberNumber, AccountTypeId));

    //                                            await context.MemberTransactionBalance.AddAsync(tb);
    //                                            await context.SaveChangesAsync();

    //                                        }



    //                                        tblReceipt rcp = new tblReceipt();

    //                                        rcp.PaymentType = payment_type;
    //                                        rcp.PaymentId = md.MemberSavingId;
    //                                        rcp.MemberNumber = md.MemberNumber;
    //                                        rcp.Description = payment_type + "-" + mem.FirstName + " " + mem.LastName;
    //                                        rcp.PaymentStatus = "Payment Confirmed";
    //                                        rcp.ReceiptAmount = md.AmountDeposited;
    //                                        rcp.ReceiptDate = md.SavingDate;
    //                                        rcp.ReceiptStaffID = StaffID;
    //                                        rcp.ReceiptNumber = GetReceiptNo();
    //                                        rcp.TotalAmountPayable = md.AmountDeposited;
    //                                        rcp.CreatedDate = md.CreatedDate;
    //                                        rcp.CreatedBy = StaffID;

    //                                        var sal = (from k in context.tblReceipt
    //                                                   where k.PaymentId == md.MemberSavingId && k.PaymentType == "Savings Payment"
    //                                                   && k.ReceiptDate == edate && k.ReceiptNumber == rcp.ReceiptNumber
    //                                                   select k).ToList().SingleOrDefault();
    //                                        if (sal == null)
    //                                        {
    //                                            await context.tblReceipt.AddAsync(rcp);
    //                                            await context.SaveChangesAsync();

    //                                        }

    //                                        try
    //                                        {
    //                                            //Insert into audittrail table
    //                                            string tableName = "MemberSaving_Deposit";
    //                                            int tableId = md.MemberSavingId;
    //                                            string opt = "Insertion";
    //                                            int empId = Convert.ToInt32(UserId);
    //                                            string fieldName = "MemberSavingId, AccountTypeId, Amount, ConfirmationStatus, MemberNumber, MemberSavingDefinitionId,SavingDate, AmountDeposited,  MemberAccountID";
    //                                            string newValue = md.MemberSavingId.ToString() + ", " + md.AccountTypeId.ToString() + ", " + md.Amount.ToString() + ", " + md.ConfirmationStatus.ToString() + ", " + md.MemberNumber.ToString() + ", " + md.MemberSavingDefinitionId.ToString() + ", " + md.SavingDate.ToString() + ", " + md.AmountDeposited.ToString() + ", " + md.MemberAccountID.ToString();

    //                                            uac.insertAtrail(tableName, opt, empId, fieldName, "", newValue);

    //                                            string dat = Convert.ToDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");


    //                                            ExecProc(tableId, tableName, opt, dat);
    //                                        }
    //                                        catch (Exception ex)
    //                                        {
    //                                            //
    //                                        }

    //                                        string Message = "Your Savings Payment is successfully initiated. You will be redirected to make an online payment of " + string.Format("{0:C}", md.AmountDeposited) + " For " + mem.FirstName + " " + mem.LastName + ".";

    //                                        response.Status = 200;
    //                                        response.Result = Message;
    //                                    }

    //                                }
    //                                else
    //                                {
    //                                    response.Status = 400;
    //                                    response.Result = "Duplicate transaction of " + string.Format("{0:C}", md.AmountDeposited) + " detected.";
    //                                }

    //                            }


    //                        }
    //                    }
    //                    else
    //                    {
    //                        response.Status = 400;
    //                        response.Result = "Member Account already exist.";
    //                    }
    //                }
    //            }
    //            else if (ops == "Edit")
    //            {
    //                ops = "Save";
    //            }
    //            else if (ops == "Save")
    //            {
    //                if (memberAccount.MemberAccountID > 0)
    //                {
    //                    int MemberAccountID = Convert.ToInt32(memberAccount.MemberAccountID);

    //                    //Check if record exist, if yes, this is the record to be updated.
    //                    var mg = (from i in context.MemberAccount
    //                              where i.MemberAccountID == MemberAccountID
    //                              select i).SingleOrDefault();

    //                    if (mg != null)
    //                    {
    //                        //mg.AccountNo = txtAccountNumber.Text;
    //                        if (!string.IsNullOrEmpty(memberAccount.InitialDeposit.ToString()))
    //                            mg.InitialDeposit = Convert.ToDecimal(memberAccount.InitialDeposit.ToString());
    //                        // mg.AccountName = Convert.ToString(txtAccountName.Text);
    //                        // mg.UserAccountNo = hfUserAccountNo.Value;
    //                        mg.AccountStatus = "Active";
    //                        mg.LockStatus = "Unlock";
    //                        mg.AccountTypeID = Convert.ToInt32(memberAccount.AccountTypeID);
    //                        mg.DateOpened = Convert.ToDateTime(memberAccount.DateOpened).Date;
    //                        mg.CreatedByStaffID = StaffID;
    //                        mg.CreatedDate = DateTime.Now.Date;
    //                        if (!string.IsNullOrEmpty(memberAccount.BVNNumber))
    //                            mg.BVNNumber = memberAccount.BVNNumber;

    //                        await context.SaveChangesAsync();

    //                        if (mg.AccountTypeID > 0)
    //                        {
    //                            response.Status = 200;
    //                            response.Result = "Record Successfully Saved";
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            //
    //        }
    //        return response;
    //    }
    //}

}
