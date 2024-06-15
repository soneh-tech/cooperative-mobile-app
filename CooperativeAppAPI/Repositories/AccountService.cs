using Azure;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace CooperativeAppAPI.Repositories
{
    public interface IAccountService
    {
        public Task<StatusResponse> AuthenticateUser(string username, string password);
        public Task<StatusResponse> Register(Member member, Staff staff);
    }
    public class AccountService : IAccountService
    {
        private readonly AppDBContext context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        private UserAuditClass uac { get; set; }


        public AccountService(AppDBContext context, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
        }
        public decimal getAccountBalance(string MemberNumber, int AcctTypeId)
        {
            decimal AccountBal = 0;

            var bal = (from i in context.MemberSavingSummary
                       join m in context.MemberAccount on i.MemberAccountID equals m.MemberAccountID
                       where i.MemberNumber == MemberNumber && m.AccountTypeID == AcctTypeId
                       select i.AccountBalance).ToList().Sum();
            if (bal != null)
            {
                if (bal > 0)
                {
                    AccountBal = Convert.ToDecimal(bal);
                }
            }


            return AccountBal;
        }
        private async void CallAutoFlagOverDuePaymentProcedure()
        {

            int RepaymentId, LoanId, ScheduleNo; DateTime PaymentDueDate, edate; string PaymentStatus, MemberNumber;
            decimal Amount = 0;
            PaymentStatus = "Overdue Payment";
            edate = DateTime.Now.Date;
            int rowCount = 0;

            var payment = (from u in context.LoanRepaymentMonthlySchedules
                           where u.PaymentDueDate < edate
                           && (u.RepaymentStatus == "Overdue Payment" || u.RepaymentStatus == "Unpaid")
                           select u).ToList().Distinct();

            //var result = await context.LoanRepaymentMonthlySchedules.ToListAsync();

            //      var payment = await context.LoanRepaymentMonthlySchedules
            //.Where(u => u.PaymentDueDate < edate && (u.RepaymentStatus == "Overdue Payment" || u.RepaymentStatus == "Unpaid"))
            //.Select(u => new
            //{
            //    u.RepaymentPlanId,
            //    u.LoanApplicationId,
            //    u.AccountTypeId,
            //    u.RepaymentPlanTypeId,
            //    u.MemberNumber,
            //    u.RemainingPrincipalBal,
            //    u.MonthRepaymentAmount,
            //    u.MonthlyPrincipalBal,
            //    u.MonthlyInterestAmount,
            //    u.PenaltyAmount,
            //    u.RepaymentScheduleNumber,
            //    u.RepaymentYear,
            //    u.RepaymentMonth,
            //    u.RepaymentDay,
            //    u.NumberofMonths,
            //    u.RepaymentStatus,
            //    PaymentDueDate = u.PaymentDueDate != null ? u.PaymentDueDate.ToString() : "",
            //    u.CreatedBy,
            //    CreatedDate = u.CreatedDate != null ? u.CreatedDate.ToString() : "",
            //    ModifiedDate = u.ModifiedDate != null ? u.ModifiedDate.ToString() : "",
            //    u.ModifiedBy,
            //    u.OrganizationId,
            //    u.ReferenceUniqueId,
            //    u.Reminder
            //})
            //.Distinct()
            //.ToListAsync();


            if (payment.Count() > 0)
            {
                foreach (var bk in payment)
                {
                    if (rowCount <= payment.Count())
                    {
                        RepaymentId = Convert.ToInt32(bk.RepaymentPlanId);
                        MemberNumber = Convert.ToString(bk.MemberNumber);
                        PaymentDueDate = Convert.ToDateTime(bk.PaymentDueDate);
                        LoanId = Convert.ToInt32(bk.LoanApplicationId);
                        PaymentStatus = bk.RepaymentStatus;


                        using (SqlConnection con = new SqlConnection())
                        {
                            con.ConnectionString = configuration.GetConnectionString("SqlConnection");
                            using (SqlCommand cmd = new SqlCommand("[dbo].[ProcAutoFlagOverDuePayment]", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@RepaymentId", SqlDbType.Int).Value = RepaymentId;
                                cmd.Parameters.Add("@MemberNumber", SqlDbType.NVarChar).Value = MemberNumber;
                                cmd.Parameters.Add("@PaymentDueDate", SqlDbType.Date).Value = PaymentDueDate;
                                cmd.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar).Value = PaymentStatus;
                                cmd.Parameters.Add("@date", SqlDbType.Date).Value = edate;
                                cmd.Parameters.Add("@LoanId", SqlDbType.Int).Value = LoanId;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }

                        }
                    }
                    rowCount = rowCount + 1;
                }
            }
        }
        private async void CallAutoCreditInterestOnSavingsProcedure()
        {

            int rate, NoDays = 0; DateTime PaymentMaturityDate, edate;
            decimal InterestAmt = 0, PrevInterestAmount = 0, myPrevAccountBalance = 0, NewAccountbalance = 0, NewAccountTotalDeposit = 0, NewInterestBal = 0;
            edate = DateTime.Now.Date;

            int rowCount = 0;

            var acct = (from u in context.AccountType select u).ToList();

            if (acct.Count() > 0)
            {
                foreach (var ac in acct)
                {
                    NoDays = Convert.ToInt32(ac.NonAccessibleTill);
                    rate = Convert.ToInt32(ac.InterestRate);

                    //Check for Business Rule for account locked for a period
                    if (NoDays > 1)
                    {
                        PaymentMaturityDate = edate.AddDays(-NoDays);

                        var payment = (from u in context.MemberSavingSummary
                                       join am in context.MemberAccount on u.MemberAccountID equals am.MemberAccountID
                                       where u.FirstSavingDate <= PaymentMaturityDate && am.AccountTypeID == ac.AccountTypeID
                                       select u).ToList().Distinct();
                        if (payment.Count() > 0)
                        {
                            foreach (var bk in payment)
                            {
                                if (rowCount <= payment.Count())
                                {
                                    var sav = (from k in context.MemberSavingSummary
                                               join am in context.MemberAccount on k.MemberAccountID equals am.MemberAccountID
                                               where am.AccountTypeID == ac.AccountTypeID
                                               && k.MemberNumber == bk.MemberNumber && k.MemberAccountID == bk.MemberAccountID
                                               && k.MemberAccountID == am.MemberAccountID
                                               select k).ToList().SingleOrDefault();
                                    if (sav != null)
                                    {
                                        //Fetch Previous Interest to add Current Interest Amount
                                        PrevInterestAmount = Convert.ToDecimal(getAccountAccumulatedInterest(sav.MemberNumber, ac.AccountTypeID));
                                        //Fetch Account balance to calculate Interest on
                                        myPrevAccountBalance = Convert.ToDecimal(getAccountBalance(sav.MemberNumber, ac.AccountTypeID));
                                        //Calculate Interest on Member's Account Balance 
                                        InterestAmt = Convert.ToDecimal(myPrevAccountBalance * rate);
                                        //Get New Interest Amount
                                        NewInterestBal = PrevInterestAmount + InterestAmt;
                                        // Get new Account balance by adding interest to the the Previous Account balance
                                        NewAccountbalance = myPrevAccountBalance + NewInterestBal;
                                        NewAccountTotalDeposit = Convert.ToDecimal(getTotalDepositAmount(sav.MemberNumber, ac.AccountTypeID)) + InterestAmt;

                                        if (NewInterestBal > 0)
                                        {
                                            var interest = new tblInterestOnAccount();
                                            interest.AccountTypeId = ac.AccountTypeID;
                                            interest.CreditAmount = InterestAmt;
                                            interest.MemberAccountID = bk.MemberAccountID;
                                            interest.MemberNumber = bk.MemberNumber;
                                            interest.CreditDate = DateTime.Now.Date;
                                            interest.Month = DateTime.Now.Month;
                                            interest.Year = DateTime.Now.Year;
                                            interest.CreditDescription = "Interest on " + ac.AccountTypeDesc + " @ " + Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                                            interest.CreditStatus = "Credit Confirmed";

                                            var ip = (from h in context.tblInterestOnAccount
                                                      where h.MemberNumber == interest.MemberNumber
                                                          && h.MemberAccountID == interest.MemberAccountID && h.AccountTypeId == interest.AccountTypeId
                                                          && h.CreditDate == interest.CreditDate && h.Month == interest.Month && h.Year == interest.Year
                                                      select h).ToList().SingleOrDefault();

                                            if (ip != null)
                                            {
                                                await context.tblInterestOnAccount.AddAsync(interest);
                                                await context.SaveChangesAsync();

                                                if (ip.InterestOnAccountId > 0)
                                                {
                                                    sav.InterestAmount = NewInterestBal;
                                                    sav.AccountBalance = NewAccountbalance;
                                                    sav.TotalDepositAmount = NewAccountTotalDeposit;

                                                    await context.SaveChangesAsync();


                                                    MemberSaving_Deposit md = new MemberSaving_Deposit();
                                                    md.AccountTypeId = ac.AccountTypeID;
                                                    md.Amount = interest.CreditAmount;
                                                    md.ConfirmationStatus = "Confirmed";
                                                    md.MemberNumber = interest.MemberNumber;
                                                    md.MemberSavingDefinitionId = null;
                                                    md.SavingDate = Convert.ToDateTime(interest.CreditDate).Date;
                                                    md.SavingDay = Convert.ToDateTime(interest.CreditDate).Day;
                                                    md.SavingMonth = Convert.ToDateTime(interest.CreditDate).Month;
                                                    md.SavingYear = Convert.ToDateTime(interest.CreditDate).Year;
                                                    md.CreatedDate = Convert.ToDateTime(interest.CreditDate).Date;
                                                    md.AmountDeposited = interest.CreditAmount;
                                                    md.MemberAccountID = interest.MemberAccountID;

                                                    //Generate unique code for every user
                                                    Guid objb = Guid.NewGuid();
                                                    string UserId = Convert.ToString(objb);
                                                    md.ReferenceUniqueId = UserId;//Unique Identifier

                                                    var suma = (from u in context.MemberSaving_Deposit
                                                                where u.MemberNumber == md.MemberNumber
                                                                && u.AccountTypeId == md.AccountTypeId
                                                                && u.MemberAccountID == md.MemberAccountID
                                                                && u.MemberNumber == md.MemberNumber
                                                                && u.SavingDate == md.SavingDate
                                                                && u.AmountDeposited == md.AmountDeposited
                                                                select u).ToList().SingleOrDefault();
                                                    if (suma == null)
                                                    {
                                                        await context.MemberSaving_Deposit.AddAsync(md);
                                                        await context.SaveChangesAsync();

                                                        var transbal = (from x in context.MemberTransactionBalance
                                                                        join am in context.MemberAccount on x.MemberAccountID equals am.MemberAccountID
                                                                        where am.AccountTypeID == md.AccountTypeId && x.MemberNumber == md.MemberNumber
                                                                        && x.MemberAccountID == md.MemberAccountID && x.TransactionDate == edate
                                                                        && x.TransactionId == md.MemberSavingId && x.TransactionType == "Credit"
                                                                        select x).ToList().SingleOrDefault();
                                                        if (transbal == null)
                                                        {
                                                            MemberTransactionBalance tb = new MemberTransactionBalance();
                                                            //Generate unique code for every user
                                                            Guid obja = Guid.NewGuid();
                                                            UserId = Convert.ToString(obja);
                                                            tb.ReferenceUniqueId = UserId;//Unique Identifier
                                                            tb.MemberNumber = md.MemberNumber;
                                                            tb.Credit = md.AmountDeposited;
                                                            tb.TransactionDate = md.SavingDate;
                                                            tb.MemberAccountID = md.MemberAccountID;
                                                            tb.TransactionId = md.MemberSavingId;
                                                            tb.TransactionType = "Credit";
                                                            tb.Description = "Interest Payment on Account";
                                                            tb.CreatedBy = 1;
                                                            tb.CreatedDate = Convert.ToDateTime(md.CreatedDate).Date;

                                                            //Set AccountBalance in daily transactional table
                                                            tb.Balance = Convert.ToDecimal(getAccountBalance(md.MemberNumber, ac.AccountTypeID));

                                                            await context.MemberTransactionBalance.AddAsync(tb);
                                                            await context.SaveChangesAsync();

                                                        }

                                                        tblReceipt rcp = new tblReceipt();
                                                        rcp.PaymentType = ac.AccountTypeDesc;
                                                        rcp.PaymentId = md.MemberSavingId;
                                                        rcp.MemberNumber = md.MemberNumber;
                                                        //rcp.Description = ac.AccountTypeDesc + "Interest Credit -" + md.Member.FirstName + " " + md.Member.LastName;
                                                        rcp.PaymentStatus = "Payment Confirmed";
                                                        rcp.ReceiptAmount = md.AmountDeposited;
                                                        rcp.ReceiptDate = md.SavingDate;
                                                        rcp.ReceiptStaffID = 1;
                                                        rcp.ReceiptNumber = GetReceiptNo();
                                                        rcp.TotalAmountPayable = md.AmountDeposited;
                                                        rcp.CreatedDate = md.CreatedDate;
                                                        rcp.CreatedBy = 1;

                                                        var sal = (from k in context.tblReceipt
                                                                   where k.PaymentId == md.MemberSavingId && k.PaymentType == "Savings Payment"
                                                                   && k.ReceiptDate == edate && k.ReceiptNumber == rcp.ReceiptNumber
                                                                   select k).ToList().SingleOrDefault();
                                                        if (sal == null)
                                                        {
                                                            await context.tblReceipt.AddAsync(rcp);
                                                            await context.SaveChangesAsync();

                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }

                                rowCount = rowCount + 1;
                            }
                        }
                    }

                }
            }
        }


        public decimal getTotalDepositAmount(string MemberNumber, int AcctTypeId)
        {
            decimal DepositAmount = 0;

            var deposit = (from i in context.MemberSavingSummary
                           join m in context.MemberAccount on i.MemberAccountID equals m.MemberAccountID
                           where i.MemberNumber == MemberNumber && m.AccountTypeID == AcctTypeId
                           select i.TotalDepositAmount).ToList().Sum();

            if (deposit > 0)
            {
                DepositAmount = Convert.ToDecimal(deposit);
            }


            return DepositAmount;
        }
        public decimal getAccountAccumulatedInterest(string MemberNumber, int AcctTypeId)
        {
            decimal AccountBal = 0;

            var bal = (from i in context.MemberSavingSummary
                       join m in context.MemberAccount on i.MemberAccountID equals m.MemberAccountID
                       where i.MemberNumber == MemberNumber && m.AccountTypeID == AcctTypeId
                       select i.InterestAmount).ToList().Sum();
            if (bal != null)
            {
                if (bal > 0)
                {
                    AccountBal = Convert.ToDecimal(bal);
                }
            }


            return AccountBal;
        }

        private async void CallAutoLoanRePaymentDebit()
        {
            int RepaymentId, LoanId, ScheduleNo; DateTime PaymentDueDate, edate; string PaymentStatus, MemberNumber;
            decimal Amount = 0, TotalAmountDue = 0;
            PaymentStatus = "Overdue Payment";
            edate = DateTime.Now.Date;
            int rowCount = 0;

            var Overduepayment = (from u in context.LoanRepaymentMonthlySchedules
                                  where u.PaymentDueDate < edate
                                  && (u.RepaymentStatus == "Overdue Payment" || u.RepaymentStatus == "Unpaid")
                                  select u).ToList().Distinct();

            //var Overduepayment = await context.LoanRepaymentMonthlySchedules
            //            .Where(u => u.PaymentDueDate < edate && (u.RepaymentStatus == "Overdue Payment" || u.RepaymentStatus == "Unpaid"))
            //            .Select(u => new
            //            {
            //                u.RepaymentPlanId,
            //                u.LoanApplicationId,
            //                u.AccountTypeId,
            //                u.RepaymentPlanTypeId,
            //                u.MemberNumber,
            //                u.RemainingPrincipalBal,
            //                u.MonthRepaymentAmount,
            //                u.MonthlyPrincipalBal,
            //                u.MonthlyInterestAmount,
            //                u.PenaltyAmount,
            //                u.RepaymentScheduleNumber,
            //                u.RepaymentYear,
            //                u.RepaymentMonth,
            //                u.RepaymentDay,
            //                u.NumberofMonths,
            //                u.RepaymentStatus,
            //                PaymentDueDate = u.PaymentDueDate != null ? u.PaymentDueDate.ToString() : "",
            //                u.CreatedBy,
            //                CreatedDate = u.CreatedDate != null ? u.CreatedDate.ToString() : "",
            //                ModifiedDate = u.ModifiedDate != null ? u.ModifiedDate.ToString() : "",
            //                u.ModifiedBy,
            //                u.OrganizationId,
            //                u.ReferenceUniqueId,
            //                u.Reminder
            //            })
            //            .Distinct()
            //            .ToListAsync();


            //var Overduepayment = await context.LoanRepaymentMonthlySchedules.Where(x=>x.PaymentDueDate < DateTime.UtcNow
            //&& (x.RepaymentStatus == "Unpaid"))
            //    .Distinct()
            //    .ToListAsync();
            if (Overduepayment.Count() > 0)
            {
                foreach (var bk in Overduepayment)
                {
                    if (rowCount <= Overduepayment.Count())
                    {
                        RepaymentId = Convert.ToInt32(bk.RepaymentPlanId);
                        MemberNumber = Convert.ToString(bk.MemberNumber);
                        PaymentDueDate = Convert.ToDateTime(bk.PaymentDueDate);
                        LoanId = Convert.ToInt32(bk.LoanApplicationId);
                        PaymentStatus = bk.RepaymentStatus;
                        Amount = Convert.ToDecimal(bk.MonthRepaymentAmount);
                        ScheduleNo = Convert.ToInt32(bk.RepaymentScheduleNumber);

                        //Lopp for Oustanding balances
                        var Repayment = (from i in context.LoanRepayment
                                         where i.PaymentStatus == "Unpaid"
                                            && i.MemberNumber == MemberNumber
                                           && i.RepaymentPlanId == RepaymentId
                                           && i.RepaymentPlanId == bk.RepaymentPlanId
                                         orderby i.RepaymentPlanId ascending
                                         select i).ToList().SingleOrDefault();
                        if (Repayment != null)
                        {
                            var account = (from l in context.MemberSavingSummary
                                           where l.MemberNumber == MemberNumber
                                           && l.AccountBalance > 0
                                           select l).ToList();
                            if (account.Count() > 0)
                            {
                                foreach (var ap in account)
                                {

                                    TotalAmountDue = Convert.ToDecimal(Repayment.OutstandingBal) + Convert.ToDecimal(Repayment.PenaltyAmount);
                                    //Auto Debit user if there is money in any account
                                    AutoLoanRePaymentDebit(MemberNumber, RepaymentId, Amount, TotalAmountDue, LoanId, PaymentDueDate, ScheduleNo);
                                }
                            }

                        }
                    }
                    rowCount = rowCount + 1;
                }
            }

            //Loop for current Month Due Repayment
            var payment = (from u in context.LoanRepaymentMonthlySchedules
                           where u.PaymentDueDate <= edate
                           && (u.RepaymentStatus == "Scheduled" || u.RepaymentStatus == "Pending")
                           select u).ToList().Distinct();
            if (payment.Count() > 0)
            {
                foreach (var bk in payment)
                {
                    if (rowCount <= payment.Count())
                    {
                        RepaymentId = Convert.ToInt32(bk.RepaymentPlanId);
                        MemberNumber = Convert.ToString(bk.MemberNumber);
                        PaymentDueDate = Convert.ToDateTime(bk.PaymentDueDate);
                        LoanId = Convert.ToInt32(bk.LoanApplicationId);
                        PaymentStatus = bk.RepaymentStatus;
                        Amount = Convert.ToDecimal(bk.MonthRepaymentAmount);
                        ScheduleNo = Convert.ToInt32(bk.RepaymentScheduleNumber);

                        //Auto Debit user if there is money in any account
                        AutoLoanRePaymentDebit(MemberNumber, RepaymentId, Amount, Amount, LoanId, PaymentDueDate, ScheduleNo);

                    }
                    rowCount = rowCount + 1;
                }
            }


        }
        protected async void AutoLoanRePaymentDebit(string MemberNumber, int RepayId, decimal Amount, decimal TotalAmtDue, int LoanId, DateTime PaymentDueDate, int RepaymentScheduleNumber)
        {
            var staff = await context.Staff.Where(x => x.RoleId == 1).FirstOrDefaultAsync();
            var StaffID = staff != null ? staff.StaffID : 0;

            decimal AmountPaid = 0, BalAmt = 0, InterestAmt = 0, TotalBal = 0, PrevTotalAmountPaid = 0, PrevTotalOutstandingBal = 0, ExpectedTotalAmt = 0;
            int AccountTypeId = 5;
            DateTime edate = Convert.ToDateTime(DateTime.Now.Date).Date;

            var bk = (from u in context.LoanRepaymentMonthlySchedules
                      where u.RepaymentPlanId == RepayId
                      select u).ToList().SingleOrDefault();
            if (bk != null)
            {
                var mem = (from i in context.Member
                           where i.MemberNumber == bk.MemberNumber
                           select i).ToList().SingleOrDefault();

                if (bk.RepaymentStatus == "Unpaid" || bk.RepaymentStatus == "Scheduled" || bk.RepaymentStatus == "Overdue Payment")
                {
                    //Execute the query here
                    AmountPaid = await getLoanAccountBalance(bk.MemberNumber, AccountTypeId, TotalAmtDue, LoanId);
                }

                //Fetch and store Previous Total AmountPaid and Outstanding Balance
                var LoanPaid = (from u in context.LoanRepaymentSummary
                                where u.MemberNumber == bk.MemberNumber
                                && u.LoanApplicationId == bk.LoanApplicationId
                                select u).ToList().SingleOrDefault();
                if (LoanPaid != null)
                {
                    PrevTotalAmountPaid = Convert.ToDecimal(LoanPaid.TotalAmountRepaid);
                    PrevTotalOutstandingBal = Convert.ToDecimal(LoanPaid.OutstandingBalance);
                }

                //Update Repayment Record
                var rom = (from k in context.LoanRepayment
                           where k.RepaymentPlanId == RepayId
                           && k.MemberNumber == bk.MemberNumber
                           && k.LoanApplicationId == bk.LoanApplicationId
                           && k.RepaymentDueDate == bk.PaymentDueDate
                           select k).ToList().SingleOrDefault();
                if (rom == null)
                {

                    LoanRepayment md = new LoanRepayment();
                    md.LoanApplicationId = Convert.ToInt32(LoanId);
                    md.MemberNumber = bk.MemberNumber;
                    md.RepaymentPlanId = bk.RepaymentPlanId;
                    md.RepaymentDueDate = PaymentDueDate;
                    md.AccountTypeId = AccountTypeId;
                    md.DatePaid = Convert.ToDateTime(edate).ToString("yyyy-MM-dd");
                    md.CreatedDate = Convert.ToDateTime(edate).Date;
                    md.CreatedBy = StaffID;
                    md.CreatedDate = Convert.ToDateTime(edate).Date;
                    md.RepaymentCount = RepaymentScheduleNumber;
                    md.RepaymentDesc = bk.LoanApplication.LoanDesc + " Loan For " + bk.RepaymentMonth + " " + bk.RepaymentYear;
                    md.RepaymentAmount = bk.MonthRepaymentAmount;
                    md.RepaymentAmountPaid = AmountPaid;
                    if (md.RepaymentAmountPaid == TotalAmtDue)
                    {
                        md.PenaltyAmount = 0;
                        md.PaymentStatus = "Paid";

                        //Update Payment Schedule table
                        if (bk != null)
                        {
                            bk.RepaymentStatus = "Paid";
                            await context.SaveChangesAsync();
                        }
                    }
                    else if (md.RepaymentAmountPaid < TotalAmtDue)
                    {
                        //Calculate Remaining balance and penalty
                        BalAmt = Convert.ToDecimal(TotalAmtDue - AmountPaid);
                        md.OutstandingBal = BalAmt;
                        md.PaymentStatus = "Unpaid";

                        //Update Payment Schedule table
                        if (bk != null)
                        {
                            bk.RepaymentStatus = "Overdue Payment";
                            await context.SaveChangesAsync();
                        }
                        //Check if member account deduct is greater or equal balanceamount, use it to offset balance amount else find the member a penalty Amount for not making full month payment
                        var LoanTypeId = (from f in context.LoanApplication
                                          where f.LoanApplicationId == bk.LoanApplicationId
                                          select f.LoanTypeId).ToList().SingleOrDefault();

                        var typ = (from a in context.LoanType
                                   where a.LoanTypeId == LoanTypeId
                                   select a).ToList().SingleOrDefault();
                        if (typ != null)
                        {
                            if (BalAmt > 0)
                            {
                                decimal rate = Convert.ToDecimal(typ.InterestRate);
                                InterestAmt = Convert.ToDecimal(rate * BalAmt);
                                md.PenaltyAmount = InterestAmt;
                                md.AnytPenalty = 1;
                            }
                        }
                        TotalBal = InterestAmt + BalAmt;

                        if (BalAmt > 0)
                        {
                            LoanPenalty pen = new LoanPenalty();
                            pen.LoanApplicationId = bk.LoanApplicationId;
                            pen.MemberNumber = bk.MemberNumber;
                            pen.OutstandingBalance = BalAmt;
                            pen.PenaltyAmount = InterestAmt;
                            pen.TotalBalance = TotalBal;
                            pen.RepaymentMonth = bk.RepaymentMonth;
                            pen.RepaymentYear = bk.RepaymentYear;

                            var pena = (from b in context.LoanPenalty
                                        where b.LoanApplicationId == pen.LoanApplicationId
                                        && b.MemberNumber == pen.MemberNumber
                                        && b.RepaymentMonth == pen.RepaymentMonth
                                        && b.RepaymentYear == pen.RepaymentYear
                                        && b.PenaltyAmount == pen.PenaltyAmount
                                        select b).ToList().SingleOrDefault();
                            if (pena == null)
                            {
                                await context.LoanPenalty.AddAsync(pen);
                                await context.SaveChangesAsync();
                            }
                        }
                    }

                    await context.LoanRepayment.AddAsync(md);
                    await context.SaveChangesAsync();

                    tblReceipt rcp = new tblReceipt();

                    if (md.RepaymentId > 0)
                    {

                        var loantype = (from u in context.LoanApplication
                                        join d in context.LoanType on u.LoanTypeId equals d.LoanTypeId
                                        where u.LoanApplicationId == bk.LoanApplicationId
                                        select u).ToList().SingleOrDefault();

                        var repayplantypeid = (from h in context.LoanRepaymentPlanType
                                               join f in context.LoanRepaymentMonthlySchedules on h.RepaymentPlanTypeId equals f.RepaymentPlanTypeId
                                               where h.RepaymentPlanTypeId == f.RepaymentPlanTypeId
                                               && f.MemberNumber == loantype.MemberNumber
                                               && f.LoanApplicationId == loantype.LoanApplicationId
                                               select h.RepaymentPlanTypeId).ToList().Take(1).SingleOrDefault();

                        var firstRepayMonth = (from ad in context.LoanRepaymentMonthlySchedules
                                               where ad.LoanApplicationId == loantype.LoanApplicationId
                                               && ad.MemberNumber == loantype.MemberNumber
                                               orderby ad.RepaymentPlanId ascending
                                               select ad).ToList().Take(1).SingleOrDefault();

                        var lasRepayMonth = (from ad in context.LoanRepaymentMonthlySchedules
                                             where ad.LoanApplicationId == loantype.LoanApplicationId
                                             && ad.MemberNumber == loantype.MemberNumber
                                             orderby ad.RepaymentPlanId descending
                                             select ad).ToList().Take(1).SingleOrDefault();

                        ExpectedTotalAmt = Convert.ToDecimal(getExpectedTotalAmount(bk.LoanApplicationId, bk.MemberNumber)) + TotalBal;

                        LoanRepaymentSummary r = new LoanRepaymentSummary();
                        r.MemberNumber = md.MemberNumber;
                        r.LoanTypeId = loantype.LoanTypeId;
                        r.LoanApplicationId = loantype.LoanApplicationId;
                        r.RepaymentPlanTypeId = repayplantypeid;
                        r.LoanDurationInMonths = loantype.ExtensionLoanPeriod;
                        r.LoanApplicationId = Convert.ToInt32(bk.LoanApplicationId);
                        r.NumberofMonthsPaid = 0;
                        r.TotalAmountRepaid = md.RepaymentAmountPaid;
                        r.TotalLoanAmount = Convert.ToDecimal(getAppliedAmount(bk.LoanApplicationId, bk.MemberNumber));
                        //r.OrganizationId = OrganizationId;
                        r.OutstandingBalance = Convert.ToDecimal(ExpectedTotalAmt - r.TotalAmountRepaid);
                        r.PaymentStatus = "SERVICING";
                        r.CreatedDate = Convert.ToDateTime(DateTime.Now.Date).Date;
                        r.CreatedBy = StaffID;
                        r.FirstRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                        r.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                        r.FirstRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                        r.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                        r.ExpectedTotalAmount = Convert.ToDecimal(ExpectedTotalAmt);
                        r.CreatedBy = StaffID;
                        r.CreatedDate = Convert.ToDateTime(DateTime.Now.Date).Date;

                        var rep = (from u in context.LoanRepaymentSummary
                                   where u.MemberNumber == r.MemberNumber
                                   && u.LoanApplicationId == r.LoanApplicationId
                                   && u.RepaymentPlanTypeId == r.RepaymentPlanTypeId
                                   && u.LoanTypeId == r.LoanTypeId
                                   select u).ToList().SingleOrDefault();
                        if (rep == null)
                        {
                            await context.LoanRepaymentSummary.AddAsync(r);
                            await context.SaveChangesAsync();

                            //Update Loan Application table
                            loantype.AmountApproved = r.TotalLoanAmount;
                            loantype.DateApproved = DateTime.Now.Date;
                            loantype.ExpectedEndDate = lasRepayMonth.PaymentDueDate;
                            loantype.InterestAmount = r.ExpectedTotalAmount - r.TotalLoanAmount;
                            loantype.PrincipalAmount = Convert.ToDecimal(r.TotalLoanAmount);
                            loantype.TotalAmount = r.ExpectedTotalAmount;
                            loantype.OutstandingBalance = loantype.TotalAmount - r.TotalAmountRepaid;
                            loantype.LoanStatus = "Approved";

                            await context.SaveChangesAsync();

                        }

                        //Deduct Loan RepaidAmount from Member's LoanAccount
                        if (md.RepaymentAmountPaid > 0)
                        {
                            string MNumber = Convert.ToString(md.MemberNumber);
                            decimal RepaymentAmountPaid = Convert.ToDecimal(md.RepaymentAmountPaid); int RepaymentId = Convert.ToInt32(md.RepaymentId);

                            DeductLoanAmountFromLoanAccount(MNumber, LoanId, AccountTypeId, RepaymentAmountPaid, RepaymentId);
                        }

                        if (rep != null)
                        {
                            ExpectedTotalAmt = Convert.ToDecimal(getExpectedTotalAmount(bk.LoanApplicationId, bk.MemberNumber)) + TotalBal;

                            if (RepaymentScheduleNumber == 1)
                            {
                                rep.FirstRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                rep.FirstRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                                rep.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                rep.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }
                            else
                            {
                                rep.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                rep.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }

                            rep.TotalAmountRepaid = getTotalAmountRepaid(bk.LoanApplicationId, bk.MemberNumber);
                            rep.NumberofMonthsPaid = md.RepaymentCount;
                            rep.ModifiedDate = Convert.ToDateTime(edate).Date;
                            rep.ModifiedBy = StaffID;
                            rep.OutstandingBalance = Convert.ToDecimal(ExpectedTotalAmt - rep.TotalAmountRepaid);

                            await context.SaveChangesAsync();


                            //Update Loan Application table
                            loantype.AmountApproved = rep.TotalLoanAmount;
                            loantype.DateApproved = DateTime.Now.Date;
                            loantype.ExpectedEndDate = lasRepayMonth.PaymentDueDate;
                            loantype.InterestAmount = rep.ExpectedTotalAmount - rep.TotalLoanAmount;
                            loantype.PrincipalAmount = Convert.ToDecimal(rep.TotalLoanAmount);
                            loantype.TotalAmount = rep.ExpectedTotalAmount;
                            loantype.OutstandingBalance = loantype.TotalAmount - rep.TotalAmountRepaid;
                            loantype.LoanStatus = "Approved";

                            await context.SaveChangesAsync();

                        }



                        var sal = (from k in context.tblReceipt
                                   where k.PaymentId == md.RepaymentId && k.PaymentType == "Loan Repayment"
                                   && k.ReceiptDate == edate
                                   select k).ToList().SingleOrDefault();
                        if (sal == null)
                        {
                            var mx = (from u in context.Member where u.MemberNumber == md.MemberNumber select u).ToList().SingleOrDefault();

                            rcp.PaymentType = "Loan Repayment";
                            rcp.PaymentId = md.RepaymentId;
                            rcp.MemberNumber = md.MemberNumber;
                            rcp.Description = md.RepaymentDesc;
                            rcp.PaymentStatus = md.PaymentStatus;
                            rcp.ReceiptAmount = md.RepaymentAmountPaid;
                            rcp.ReceiptDate = Convert.ToDateTime(md.DatePaid).Date;
                            rcp.ReceiptStaffID = StaffID;
                            rcp.ReceiptNumber = GetReceiptNo();
                            rcp.TotalAmountPayable = md.RepaymentAmount;
                            rcp.CreatedDate = md.CreatedDate;
                            rcp.CreatedBy = StaffID;

                            await context.tblReceipt.AddAsync(rcp);
                            await context.SaveChangesAsync();

                        }

                        try
                        {
                            if (md.RepaymentId > 0)
                            {
                                var act = (from n in context.MemberAccount where n.AccountTypeID == 5 && n.MemberNumber == mem.MemberNumber select n).ToList().SingleOrDefault();

                                var method = (from sp in context.tblNotificationMethod select sp).Take(1).ToList().SingleOrDefault();
                                if (method != null)
                                {
                                    if (method.NotificationMethodDesc == "SMS" || method.NotificationMethodDesc == "Email and SMS")
                                    {
                                        if (!string.IsNullOrEmpty(mem.Mobile))
                                        {
                                            string WebsiteURL = null; String strPathAndQuery = null;
                                            strPathAndQuery = "HttpContext.Current.Request.Url.PathAndQuery";
                                            WebsiteURL = hostingEnvironment.WebRootPath;

                                            //You call the class by using
                                            //Send sms
                                            string msg = "Loan Repayment Debit" + "\n" + "Amt:NGN" + string.Format("{0:N}", md.RepaymentAmountPaid) + "\n" + "Acc:" + act.AccountNo + "\n" + "Desc: " + Convert.ToDateTime(edate).ToString("MMM-yyyy") + "\n" + " Date: " + Convert.ToDateTime(md.DatePaid).ToString("dd/MM/yyyy") + "\n" + "Avail Bal:NGN" + string.Format("{0:N}", Convert.ToDecimal(getAccountBalance(md.MemberNumber, AccountTypeId))) + "\n" + "(QUEENS MPCS)" + Environment.NewLine;
                                            SMSMember(mem.MemberNumber, mem.Mobile, msg);
                                        }

                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        try
                        {

                            //Insert into audittrail table
                            string tableName = "LoanRepayment";
                            int tableId = md.RepaymentId;
                            string opt = "Insertion";
                            int empId = Convert.ToInt32(StaffID);
                            string fieldName = "RepaymentId, LoanApplicationId, RepaymentAmount, RepaymentPlanId, MemberNumber, DatePaid,PaymentStatus, RepaymentDesc,  RepaymentCount";
                            string newValue = md.RepaymentId.ToString() + ", " + md.LoanApplicationId.ToString() + ", " + md.RepaymentAmount.ToString() + ", " + md.RepaymentPlanId.ToString() + ", " + md.MemberNumber.ToString() + ", " + md.DatePaid.ToString() + ", " + md.PaymentStatus.ToString() + ", " + md.RepaymentDesc.ToString() + ", " + md.RepaymentCount.ToString();

                            uac.insertAtrail(tableName, opt, empId, fieldName, "", newValue);

                        }
                        catch (Exception ex)
                        {
                            //
                        }


                    }

                }
                else
                {
                    //Get previous amount paid
                    decimal PrevPayment = Convert.ToDecimal(rom.RepaymentAmountPaid);
                    DateTime lastPaymentDueDate = Convert.ToDateTime(rom.RepaymentDueDate).Date;

                    //if amountPaid is greater than 0 and it is not yet due date
                    if (lastPaymentDueDate.AddMonths(1) > edate && AmountPaid > 0)
                    {
                        //Update Previous Incomplete Loan Repayment
                        if (AmountPaid == TotalAmtDue)
                        {
                            rom.PenaltyAmount = 0;
                            rom.RepaymentAmountPaid = AmountPaid + PrevPayment;
                            rom.PaymentStatus = "Paid";

                            //Update Payment Schedule table
                            if (bk != null)
                            {
                                bk.RepaymentStatus = "Paid";
                                await context.SaveChangesAsync();
                            }
                        }
                        else if (AmountPaid < TotalAmtDue)
                        {
                            //Calculate Remaining balance and penalty
                            BalAmt = Convert.ToDecimal(TotalAmtDue - AmountPaid);
                            rom.OutstandingBal = BalAmt;
                            rom.RepaymentAmountPaid = AmountPaid + PrevPayment;
                            rom.PaymentStatus = "Unpaid";

                            //Update Payment Schedule table
                            if (bk != null)
                            {
                                bk.RepaymentStatus = "Overdue Payment";
                                await context.SaveChangesAsync();
                            }

                            //Check if member account deduct is greater or equal balanceamount, use it to offset balance amount else find the member a penalty Amount for not making full month payment
                            var LoanTypeId = (from f in context.LoanApplication
                                              where f.LoanApplicationId == bk.LoanApplicationId
                                              select f.LoanTypeId).ToList().SingleOrDefault();
                            if (lastPaymentDueDate.AddMonths(1) <= edate)
                            {
                                var typ = (from a in context.LoanType
                                           where a.LoanTypeId == LoanTypeId
                                           select a).ToList().SingleOrDefault();
                                if (typ != null)
                                {
                                    if (BalAmt > 0)
                                    {
                                        decimal rate = Convert.ToDecimal(typ.InterestRate);
                                        InterestAmt = Convert.ToDecimal(rate * BalAmt);

                                        rom.PenaltyAmount = InterestAmt;
                                        rom.AnytPenalty = 1;
                                    }
                                }
                                TotalBal = InterestAmt + BalAmt;

                                if (BalAmt > 0)
                                {
                                    LoanPenalty pen = new LoanPenalty();
                                    pen.LoanApplicationId = bk.LoanApplicationId;
                                    pen.MemberNumber = bk.MemberNumber;
                                    pen.OutstandingBalance = BalAmt;
                                    pen.PenaltyAmount = InterestAmt;
                                    pen.TotalBalance = TotalBal;
                                    pen.RepaymentMonth = bk.RepaymentMonth;
                                    pen.RepaymentYear = bk.RepaymentYear;

                                    var pena = (from b in context.LoanPenalty
                                                where b.LoanApplicationId == pen.LoanApplicationId
                                                && b.MemberNumber == pen.MemberNumber
                                                && b.RepaymentMonth == pen.RepaymentMonth
                                                && b.RepaymentYear == pen.RepaymentYear
                                                && b.PenaltyAmount == pen.PenaltyAmount
                                                select b).ToList().SingleOrDefault();
                                    if (pena == null)
                                    {
                                        await context.LoanPenalty.AddAsync(pen);
                                        await context.SaveChangesAsync();
                                    }
                                }
                            }

                            TotalBal = InterestAmt + BalAmt;
                        }


                        await context.SaveChangesAsync();

                        var repa = (from u in context.LoanRepaymentSummary
                                    where u.MemberNumber == rom.MemberNumber
                                    && u.LoanApplicationId == rom.LoanApplicationId
                                    select u).ToList().SingleOrDefault();
                        if (repa != null)
                        {
                            ExpectedTotalAmt = Convert.ToDecimal(getExpectedTotalAmount(bk.LoanApplicationId, bk.MemberNumber)) + TotalBal;

                            if (RepaymentScheduleNumber == 1)
                            {
                                repa.FirstRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.FirstRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                                repa.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }
                            else
                            {
                                repa.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }

                            repa.TotalAmountRepaid = getTotalAmountRepaid(bk.LoanApplicationId, bk.MemberNumber);
                            repa.NumberofMonthsPaid = rom.RepaymentCount;
                            repa.ModifiedDate = Convert.ToDateTime(edate).Date;
                            repa.ModifiedBy = StaffID;
                            repa.OutstandingBalance = Convert.ToDecimal(ExpectedTotalAmt - repa.TotalAmountRepaid);

                            await context.SaveChangesAsync();


                            var loantype = (from u in context.LoanApplication
                                            join d in context.LoanType on u.LoanTypeId equals d.LoanTypeId
                                            where u.LoanApplicationId == bk.LoanApplicationId
                                            select u).ToList().SingleOrDefault();
                            if (loantype != null)
                            {
                                //Update Loan Application table
                                loantype.AmountApproved = repa.TotalLoanAmount;
                                loantype.DateApproved = DateTime.Now.Date;
                                loantype.ExpectedEndDate = bk.PaymentDueDate;
                                loantype.InterestAmount = repa.ExpectedTotalAmount - repa.TotalLoanAmount;
                                loantype.PrincipalAmount = Convert.ToDecimal(repa.TotalLoanAmount);
                                loantype.TotalAmount = repa.ExpectedTotalAmount;
                                loantype.OutstandingBalance = loantype.TotalAmount - repa.TotalAmountRepaid;
                                loantype.LoanStatus = "Approved";

                                await context.SaveChangesAsync();
                            }

                            //Deduct Loan RepaidAmount from Member's LoanAccount
                            if (rom.RepaymentAmountPaid > 0)
                            {
                                string MNumber = Convert.ToString(rom.MemberNumber);
                                decimal RepaymentAmountPaid = Convert.ToDecimal(AmountPaid); int RepaymentId = Convert.ToInt32(rom.RepaymentId);

                                DeductLoanAmountFromLoanAccount(MNumber, LoanId, AccountTypeId, RepaymentAmountPaid, RepaymentId);
                            }

                            var sal = (from k in context.tblReceipt
                                       where k.PaymentId == rom.RepaymentId && k.PaymentType == "Loan Repayment"
                                       && k.ReceiptDate == edate
                                       select k).ToList().SingleOrDefault();
                            if (sal == null)
                            {
                                var mx = (from u in context.Member where u.MemberNumber == rom.MemberNumber select u).ToList().SingleOrDefault();
                                tblReceipt rcp = new tblReceipt();
                                rcp.PaymentType = "Loan Repayment";
                                rcp.PaymentId = rom.RepaymentId;
                                rcp.MemberNumber = rom.MemberNumber;
                                rcp.Description = rom.RepaymentDesc;
                                rcp.PaymentStatus = rom.PaymentStatus;
                                rcp.ReceiptAmount = AmountPaid;
                                rcp.ReceiptDate = Convert.ToDateTime(rom.DatePaid).Date;
                                rcp.ReceiptStaffID = StaffID;
                                rcp.ReceiptNumber = GetReceiptNo();
                                rcp.TotalAmountPayable = TotalAmtDue;
                                rcp.CreatedDate = rom.CreatedDate;
                                rcp.CreatedBy = StaffID;

                                await context.tblReceipt.AddAsync(rcp);
                                await context.SaveChangesAsync();

                            }

                        }
                    }
                    //if amountPaid equals 0 but it is another month due date
                    else if (lastPaymentDueDate.AddMonths(1) == edate)
                    {
                        //Update Previous Incomplete Loan Repayment
                        if (AmountPaid == TotalAmtDue)
                        {
                            rom.PenaltyAmount = 0;
                            rom.RepaymentAmountPaid = AmountPaid + PrevPayment;
                            rom.PaymentStatus = "Paid";

                            //Update Payment Schedule table
                            if (bk != null)
                            {
                                bk.RepaymentStatus = "Paid";
                                await context.SaveChangesAsync();
                            }
                        }
                        else if (AmountPaid < TotalAmtDue)
                        {
                            //Calculate Remaining balance and penalty
                            BalAmt = Convert.ToDecimal(TotalAmtDue - AmountPaid);
                            rom.OutstandingBal = BalAmt;
                            rom.RepaymentAmountPaid = AmountPaid + PrevPayment;
                            rom.PaymentStatus = "Unpaid";

                            //Update Payment Schedule table
                            if (bk != null)
                            {
                                bk.RepaymentStatus = "Overdue Payment";
                                await context.SaveChangesAsync();
                            }

                            //Check if member account deduct is greater or equal balanceamount, use it to offset balance amount else find the member a penalty Amount for not making full month payment
                            var LoanTypeId = (from f in context.LoanApplication
                                              where f.LoanApplicationId == bk.LoanApplicationId
                                              select f.LoanTypeId).ToList().SingleOrDefault();
                            if (lastPaymentDueDate.AddMonths(1) <= edate)
                            {
                                var typ = (from a in context.LoanType
                                           where a.LoanTypeId == LoanTypeId
                                           select a).ToList().SingleOrDefault();
                                if (typ != null)
                                {
                                    if (BalAmt > 0)
                                    {
                                        decimal rate = Convert.ToDecimal(typ.InterestRate);
                                        InterestAmt = Convert.ToDecimal(rate * BalAmt);

                                        rom.PenaltyAmount = InterestAmt;
                                        rom.AnytPenalty = 1;
                                    }
                                }
                                TotalBal = InterestAmt + BalAmt;

                                if (BalAmt > 0)
                                {
                                    LoanPenalty pen = new LoanPenalty();
                                    pen.LoanApplicationId = bk.LoanApplicationId;
                                    pen.MemberNumber = bk.MemberNumber;
                                    pen.OutstandingBalance = BalAmt;
                                    pen.PenaltyAmount = InterestAmt;
                                    pen.TotalBalance = TotalBal;
                                    pen.RepaymentMonth = bk.RepaymentMonth;
                                    pen.RepaymentYear = bk.RepaymentYear;

                                    var pena = (from b in context.LoanPenalty
                                                where b.LoanApplicationId == pen.LoanApplicationId
                                                && b.MemberNumber == pen.MemberNumber
                                                && b.RepaymentMonth == pen.RepaymentMonth
                                                && b.RepaymentYear == pen.RepaymentYear
                                                && b.PenaltyAmount == pen.PenaltyAmount
                                                select b).ToList().SingleOrDefault();
                                    if (pena == null)
                                    {
                                        await context.LoanPenalty.AddAsync(pen);
                                        await context.SaveChangesAsync();
                                    }
                                }
                            }

                            TotalBal = InterestAmt + BalAmt;
                        }


                        await context.SaveChangesAsync();

                        var repa = (from u in context.LoanRepaymentSummary
                                    where u.MemberNumber == rom.MemberNumber
                                    && u.LoanApplicationId == rom.LoanApplicationId
                                    select u).ToList().SingleOrDefault();
                        if (repa != null)
                        {
                            ExpectedTotalAmt = Convert.ToDecimal(getExpectedTotalAmount(bk.LoanApplicationId, bk.MemberNumber)) + TotalBal;

                            if (RepaymentScheduleNumber == 1)
                            {
                                repa.FirstRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.FirstRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                                repa.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }
                            else
                            {
                                repa.LastRepaymentMonth = Convert.ToDateTime(PaymentDueDate).Month;
                                repa.LastRepaymentYear = Convert.ToDateTime(PaymentDueDate).Year;
                            }

                            repa.TotalAmountRepaid = getTotalAmountRepaid(bk.LoanApplicationId, bk.MemberNumber);
                            repa.NumberofMonthsPaid = rom.RepaymentCount;
                            repa.ModifiedDate = Convert.ToDateTime(edate).Date;
                            repa.ModifiedBy = StaffID;
                            repa.OutstandingBalance = Convert.ToDecimal(ExpectedTotalAmt - repa.TotalAmountRepaid);

                            await context.SaveChangesAsync();


                            var loantype = (from u in context.LoanApplication
                                            join d in context.LoanType on u.LoanTypeId equals d.LoanTypeId
                                            where u.LoanApplicationId == bk.LoanApplicationId
                                            select u).ToList().SingleOrDefault();
                            if (loantype != null)
                            {
                                //Update Loan Application table
                                loantype.AmountApproved = repa.TotalLoanAmount;
                                loantype.DateApproved = DateTime.Now.Date;
                                loantype.ExpectedEndDate = bk.PaymentDueDate;
                                loantype.InterestAmount = repa.ExpectedTotalAmount - repa.TotalLoanAmount;
                                loantype.PrincipalAmount = Convert.ToDecimal(repa.TotalLoanAmount);
                                loantype.TotalAmount = repa.ExpectedTotalAmount;
                                loantype.OutstandingBalance = loantype.TotalAmount - repa.TotalAmountRepaid;
                                loantype.LoanStatus = "Approved";

                                await context.SaveChangesAsync();
                            }

                            //Deduct Loan RepaidAmount from Member's LoanAccount
                            if (rom.RepaymentAmountPaid > 0)
                            {
                                string MNumber = Convert.ToString(rom.MemberNumber);
                                decimal RepaymentAmountPaid = Convert.ToDecimal(AmountPaid); int RepaymentId = Convert.ToInt32(rom.RepaymentId);

                                DeductLoanAmountFromLoanAccount(MNumber, LoanId, AccountTypeId, RepaymentAmountPaid, RepaymentId);
                            }

                            var sal = (from k in context.tblReceipt
                                       where k.PaymentId == rom.RepaymentId && k.PaymentType == "Loan Repayment"
                                       && k.ReceiptDate == edate
                                       select k).ToList().SingleOrDefault();
                            if (sal == null)
                            {
                                var mx = (from u in context.Member where u.MemberNumber == rom.MemberNumber select u).ToList().SingleOrDefault();
                                tblReceipt rcp = new tblReceipt();
                                rcp.PaymentType = "Loan Repayment";
                                rcp.PaymentId = rom.RepaymentId;
                                rcp.MemberNumber = rom.MemberNumber;
                                rcp.Description = rom.RepaymentDesc;
                                rcp.PaymentStatus = rom.PaymentStatus;
                                rcp.ReceiptAmount = AmountPaid;
                                rcp.ReceiptDate = Convert.ToDateTime(rom.DatePaid).Date;
                                rcp.ReceiptStaffID = StaffID;
                                rcp.ReceiptNumber = GetReceiptNo();
                                rcp.TotalAmountPayable = TotalAmtDue;
                                rcp.CreatedDate = rom.CreatedDate;
                                rcp.CreatedBy = StaffID;

                                await context.tblReceipt.AddAsync(rcp);
                                await context.SaveChangesAsync();

                            }

                        }
                    }

                }
            }
            //}
        }
        public async Task<decimal> getLoanAccountBalance(string MemberNumber, int AcctTypeId, decimal AmountDue, int LoanId)
        {
            decimal AccountBal = 0, CurrentBalAmt = 0, differenceAmt = 0; int iCount = 0;

            var staff = await context.Staff.Where(a => a.RoleId == 1).FirstOrDefaultAsync();
            var StaffID = staff != null ? staff.StaffID : 0;

            var MemberAccountID = (from la in context.MemberAccount
                                   where la.MemberNumber == MemberNumber
                                   && la.AccountTypeID == 5
                                   select la.MemberAccountID).ToList().SingleOrDefault();

            MemberSavingSummary pm = new MemberSavingSummary();
            pm.MemberNumber = MemberNumber;
            pm.LastSavingAmount = 0;
            pm.LastSavingDate = DateTime.Now.Date;
            pm.FirstSavingAmount = 0;
            pm.FirstSavingDate = DateTime.Now.Date;
            pm.MemberAccountID = MemberAccountID;
            pm.AccountStatus = "Active";
            pm.CreatedBy = StaffID;
            pm.CreatedDate = DateTime.Now.Date;
            pm.AccountBalance = 0;
            pm.TotalDepositAmount = 0;
            //Generate unique code for every user
            Guid objj = Guid.NewGuid();
            pm.ReferenceUniqueId = Convert.ToString(objj);//Unique Identifier
            var pss = (from x in context.MemberSavingSummary
                       join am in context.MemberAccount on x.MemberAccountID equals am.MemberAccountID
                       where am.AccountTypeID == AcctTypeId &&
                       x.MemberNumber == MemberNumber
                       && x.MemberAccountID == MemberAccountID
                       select x).ToList().SingleOrDefault();
            if (pss == null)
            {
                await context.MemberSavingSummary.AddAsync(pm);
                await context.SaveChangesAsync();
            }

            var loanacct = (from i in context.MemberSavingSummary
                            join m in context.MemberAccount on i.MemberAccountID equals m.MemberAccountID
                            where i.MemberNumber == MemberNumber && m.AccountTypeID == AcctTypeId
                            select i).ToList().SingleOrDefault();
            if (loanacct != null)
            {
                if (loanacct.AccountBalance > 0)
                {
                    if (loanacct.AccountBalance >= AmountDue)
                    {
                        AccountBal = Convert.ToDecimal(AmountDue);
                    }
                    else
                    {
                        //get the difference
                        CurrentBalAmt = Convert.ToDecimal(loanacct.AccountBalance);
                        differenceAmt = AmountDue - CurrentBalAmt;

                        //get other accounts with Balance
                        var macct = (from a in context.MemberAccount
                                     join ss in context.MemberSavingSummary on a.MemberAccountID equals ss.MemberAccountID
                                     where a.MemberNumber == MemberNumber
                                     && a.AccountTypeID != AcctTypeId && ss.AccountBalance > 0
                                     select a).ToList();
                        if (macct.Count() > 0)
                        {

                            foreach (var a in macct)
                            {
                                var money = (from i in context.MemberSavingSummary
                                             where i.MemberNumber == MemberNumber && i.MemberAccountID == a.MemberAccountID
                                             select i).ToList().SingleOrDefault();
                                if (money.AccountBalance >= differenceAmt)
                                {
                                    if (iCount == 0)
                                    {
                                        if (differenceAmt > 0)
                                        {
                                            loanacct.AccountBalance = differenceAmt + CurrentBalAmt;
                                            loanacct.LastSavingDate = DateTime.Now.Date;
                                            await context.SaveChangesAsync();

                                            //Subtract from Savings table
                                            money.AccountBalance = money.AccountBalance - differenceAmt;
                                            await context.SaveChangesAsync();
                                            iCount = iCount + 1;

                                            AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                            Guid obj = Guid.NewGuid();
                                            MemberWithdrawal m = new MemberWithdrawal();
                                            m.MemberNumber = MemberNumber;
                                            m.MemberAccountID = a.MemberAccountID;
                                            m.WithdrawalDesc = "Loan Repayment Transfer";
                                            m.WithdrawalAmount = Convert.ToDecimal(differenceAmt);
                                            m.WithdrawalRequestedDate = DateTime.Now.Date;
                                            m.WithdrawalDate = DateTime.Now.Date;
                                            m.WithdrawalStatus = "Approved";
                                            m.WithdrawalType = "Transfer";
                                            m.CreatedDate = DateTime.Now.Date;
                                            m.CreatedByUserRole = "Staff";
                                            m.CreatedBy = StaffID;
                                            m.ReferenceUniqueId = Convert.ToString(obj);

                                            if (m.WithdrawalAmount > 0)
                                            {
                                                var chk = (from i in context.MemberWithdrawal
                                                           where i.WithdrawalDesc == m.WithdrawalDesc
                                                           && i.WithdrawalAmount == m.WithdrawalAmount
                                                           && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                           && i.MemberNumber == m.MemberNumber
                                                           && i.MemberAccountID == m.MemberAccountID
                                                           select i).ToList().SingleOrDefault();
                                                if (chk == null)
                                                {
                                                    await context.MemberWithdrawal.AddAsync(m);
                                                    await context.SaveChangesAsync();
                                                }
                                            }
                                        }

                                    }

                                }
                                else
                                {
                                    CurrentBalAmt = Convert.ToDecimal(loanacct.AccountBalance);
                                    differenceAmt = AmountDue - CurrentBalAmt;

                                    //if (iCount == 0)
                                    //{
                                    if (money.AccountBalance >= differenceAmt)
                                    {
                                        loanacct.AccountBalance = differenceAmt + CurrentBalAmt;
                                        loanacct.LastSavingDate = DateTime.Now.Date;
                                        await context.SaveChangesAsync();

                                        //Subtract from Savings table
                                        money.AccountBalance = money.AccountBalance - differenceAmt;
                                        await context.SaveChangesAsync();
                                        iCount = iCount + 1;

                                        AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                        Guid obj = Guid.NewGuid();
                                        MemberWithdrawal m = new MemberWithdrawal();
                                        m.MemberNumber = MemberNumber;
                                        m.MemberAccountID = a.MemberAccountID;
                                        m.WithdrawalDesc = "Loan Repayment Transfer";
                                        m.WithdrawalAmount = Convert.ToDecimal(AccountBal);
                                        m.WithdrawalRequestedDate = DateTime.Now.Date;
                                        m.WithdrawalDate = DateTime.Now.Date;
                                        m.WithdrawalStatus = "Approved";
                                        m.WithdrawalType = "Transfer";
                                        m.CreatedDate = DateTime.Now.Date;
                                        m.CreatedByUserRole = "Staff";
                                        m.CreatedBy = StaffID;
                                        m.ReferenceUniqueId = Convert.ToString(obj);

                                        if (m.WithdrawalAmount > 0)
                                        {
                                            var chk = (from i in context.MemberWithdrawal
                                                       where i.WithdrawalDesc == m.WithdrawalDesc
                                                       && i.WithdrawalAmount == m.WithdrawalAmount
                                                       && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                       && i.MemberNumber == m.MemberNumber
                                                       && i.MemberAccountID == m.MemberAccountID
                                                       select i).ToList().SingleOrDefault();
                                            if (chk == null)
                                            {
                                                await context.MemberWithdrawal.AddAsync(m);
                                                await context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        loanacct.AccountBalance = money.AccountBalance + CurrentBalAmt;
                                        loanacct.LastSavingDate = DateTime.Now.Date;
                                        await context.SaveChangesAsync();

                                        //Subtract from Savings table
                                        money.AccountBalance = 0;
                                        await context.SaveChangesAsync();
                                        iCount = iCount + 1;

                                        AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                        Guid obj = Guid.NewGuid();
                                        MemberWithdrawal m = new MemberWithdrawal();
                                        m.MemberNumber = MemberNumber;
                                        m.MemberAccountID = a.MemberAccountID;
                                        m.WithdrawalDesc = "Loan Repayment Transfer";
                                        m.WithdrawalAmount = Convert.ToDecimal(AccountBal);
                                        m.WithdrawalRequestedDate = DateTime.Now.Date;
                                        m.WithdrawalDate = DateTime.Now.Date;
                                        m.WithdrawalStatus = "Approved";
                                        m.WithdrawalType = "Transfer";
                                        m.CreatedDate = DateTime.Now.Date;
                                        m.CreatedByUserRole = "Staff";
                                        m.CreatedBy = StaffID;
                                        m.ReferenceUniqueId = Convert.ToString(obj);

                                        if (m.WithdrawalAmount > 0)
                                        {
                                            var chk = (from i in context.MemberWithdrawal
                                                       where i.WithdrawalDesc == m.WithdrawalDesc
                                                       && i.WithdrawalAmount == m.WithdrawalAmount
                                                       && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                       && i.MemberNumber == m.MemberNumber
                                                       && i.MemberAccountID == m.MemberAccountID
                                                       select i).ToList().SingleOrDefault();
                                            if (chk == null)
                                            {
                                                await context.MemberWithdrawal.AddAsync(m);
                                                await context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    //}
                                }
                            }
                        }
                    }
                    AccountBal = Convert.ToDecimal(loanacct.AccountBalance);
                }
                else //When the user has zero balance in the Loan Account
                {
                    //get other accounts with Balance
                    var macct = (from a in context.MemberAccount
                                 join ss in context.MemberSavingSummary on a.MemberAccountID equals ss.MemberAccountID
                                 where a.MemberNumber == MemberNumber
                                 && a.AccountTypeID != AcctTypeId && ss.AccountBalance > 0
                                 select a).ToList();
                    if (macct.Count() > 0)
                    {
                        foreach (var a in macct)
                        {
                            CurrentBalAmt = Convert.ToDecimal(loanacct.AccountBalance);
                            differenceAmt = AmountDue - CurrentBalAmt;

                            var money = (from i in context.MemberSavingSummary
                                         where i.MemberNumber == MemberNumber && i.MemberAccountID == a.MemberAccountID
                                         select i).ToList().SingleOrDefault();

                            if (money.AccountBalance >= differenceAmt)
                            {
                                if (iCount == 0)
                                {
                                    loanacct.AccountBalance = differenceAmt + CurrentBalAmt;
                                    loanacct.LastSavingDate = DateTime.Now.Date;
                                    await context.SaveChangesAsync();

                                    //Subtract from Savings table
                                    money.AccountBalance = money.AccountBalance - differenceAmt;
                                    await context.SaveChangesAsync();

                                    iCount = iCount + 1;

                                    AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                    Guid obj = Guid.NewGuid();
                                    MemberWithdrawal m = new MemberWithdrawal();
                                    m.MemberNumber = MemberNumber;
                                    m.MemberAccountID = a.MemberAccountID;
                                    m.WithdrawalDesc = "Loan Repayment Transfer";
                                    m.WithdrawalAmount = Convert.ToDecimal(differenceAmt);
                                    m.WithdrawalRequestedDate = DateTime.Now.Date;
                                    m.WithdrawalDate = DateTime.Now.Date;
                                    m.WithdrawalStatus = "Approved";
                                    m.WithdrawalType = "Transfer";
                                    m.CreatedDate = DateTime.Now.Date;
                                    m.CreatedByUserRole = "Staff";
                                    m.CreatedBy = StaffID;
                                    m.ReferenceUniqueId = Convert.ToString(obj);

                                    if (m.WithdrawalAmount > 0)
                                    {
                                        var chk = (from i in context.MemberWithdrawal
                                                   where i.WithdrawalDesc == m.WithdrawalDesc
                                                   && i.WithdrawalAmount == m.WithdrawalAmount
                                                   && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                   && i.MemberNumber == m.MemberNumber
                                                   && i.MemberAccountID == m.MemberAccountID
                                                   select i).ToList().SingleOrDefault();
                                        if (chk == null)
                                        {
                                            await context.MemberWithdrawal.AddAsync(m);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                }
                                else
                                {
                                    if (money.AccountBalance >= differenceAmt)
                                    {
                                        loanacct.AccountBalance = CurrentBalAmt + differenceAmt;
                                        loanacct.LastSavingDate = DateTime.Now.Date;
                                        await context.SaveChangesAsync();

                                        //Subtract from Savings table
                                        money.AccountBalance = money.AccountBalance - differenceAmt;
                                        await context.SaveChangesAsync();

                                        iCount = iCount + 1;

                                        AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                        Guid obj = Guid.NewGuid();
                                        MemberWithdrawal m = new MemberWithdrawal();
                                        m.MemberNumber = MemberNumber;
                                        m.MemberAccountID = a.MemberAccountID;
                                        m.WithdrawalDesc = "Loan Repayment Transfer";
                                        m.WithdrawalAmount = Convert.ToDecimal(differenceAmt);
                                        m.WithdrawalRequestedDate = DateTime.Now.Date;
                                        m.WithdrawalDate = DateTime.Now.Date;
                                        m.WithdrawalStatus = "Approved";
                                        m.WithdrawalType = "Transfer";
                                        m.CreatedDate = DateTime.Now.Date;
                                        m.CreatedByUserRole = "Staff";
                                        m.CreatedBy = StaffID;
                                        m.ReferenceUniqueId = Convert.ToString(obj);

                                        if (m.WithdrawalAmount > 0)
                                        {
                                            var chk = (from i in context.MemberWithdrawal
                                                       where i.WithdrawalDesc == m.WithdrawalDesc
                                                       && i.WithdrawalAmount == m.WithdrawalAmount
                                                       && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                       && i.MemberNumber == m.MemberNumber
                                                       && i.MemberAccountID == m.MemberAccountID
                                                       select i).ToList().SingleOrDefault();
                                            if (chk == null)
                                            {
                                                await context.MemberWithdrawal.AddAsync(m);
                                                await context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        loanacct.AccountBalance = CurrentBalAmt + money.AccountBalance;
                                        loanacct.LastSavingDate = DateTime.Now.Date;
                                        await context.SaveChangesAsync();

                                        //Subtract from Savings table
                                        money.AccountBalance = 0;
                                        await context.SaveChangesAsync();

                                        iCount = iCount + 1;

                                        AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                        Guid obj = Guid.NewGuid();
                                        MemberWithdrawal m = new MemberWithdrawal();
                                        m.MemberNumber = MemberNumber;
                                        m.MemberAccountID = a.MemberAccountID;
                                        m.WithdrawalDesc = "Loan Repayment Transfer";
                                        m.WithdrawalAmount = Convert.ToDecimal(money.AccountBalance);
                                        m.WithdrawalRequestedDate = DateTime.Now.Date;
                                        m.WithdrawalDate = DateTime.Now.Date;
                                        m.WithdrawalStatus = "Approved";
                                        m.WithdrawalType = "Transfer";
                                        m.CreatedDate = DateTime.Now.Date;
                                        m.CreatedByUserRole = "Staff";
                                        m.CreatedBy = StaffID;
                                        m.ReferenceUniqueId = Convert.ToString(obj);

                                        if (m.WithdrawalAmount > 0)
                                        {
                                            var chk = (from i in context.MemberWithdrawal
                                                       where i.WithdrawalDesc == m.WithdrawalDesc
                                                       && i.WithdrawalAmount == m.WithdrawalAmount
                                                       && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                                       && i.MemberNumber == m.MemberNumber
                                                       && i.MemberAccountID == m.MemberAccountID
                                                       select i).ToList().SingleOrDefault();
                                            if (chk == null)
                                            {
                                                await context.MemberWithdrawal.AddAsync(m);
                                                await context.SaveChangesAsync();
                                            }
                                        }
                                    }

                                }

                            }
                            else
                            {
                                loanacct.AccountBalance = CurrentBalAmt + money.AccountBalance;
                                loanacct.LastSavingDate = DateTime.Now.Date;
                                await context.SaveChangesAsync();

                                //Subtract from Savings table
                                money.AccountBalance = 0;
                                await context.SaveChangesAsync();

                                iCount = iCount + 1;

                                AccountBal = Convert.ToDecimal(loanacct.AccountBalance);

                                Guid obj = Guid.NewGuid();
                                MemberWithdrawal m = new MemberWithdrawal();
                                m.MemberNumber = MemberNumber;
                                m.MemberAccountID = a.MemberAccountID;
                                m.WithdrawalDesc = "Loan Repayment Transfer";
                                m.WithdrawalAmount = Convert.ToDecimal(money.AccountBalance);
                                m.WithdrawalRequestedDate = DateTime.Now.Date;
                                m.WithdrawalDate = DateTime.Now.Date;
                                m.WithdrawalStatus = "Approved";
                                m.WithdrawalType = "Transfer";
                                m.CreatedDate = DateTime.Now.Date;
                                m.CreatedByUserRole = "Staff";
                                m.CreatedBy = StaffID;
                                m.ReferenceUniqueId = Convert.ToString(obj);

                                if (m.WithdrawalAmount > 0)
                                {
                                    var chk = (from i in context.MemberWithdrawal
                                               where i.WithdrawalDesc == m.WithdrawalDesc
                                               && i.WithdrawalAmount == m.WithdrawalAmount
                                               && i.WithdrawalRequestedDate == m.WithdrawalRequestedDate
                                               && i.MemberNumber == m.MemberNumber
                                               && i.MemberAccountID == m.MemberAccountID
                                               select i).ToList().SingleOrDefault();
                                    if (chk == null)
                                    {
                                        await context.MemberWithdrawal.AddAsync(m);
                                        await context.SaveChangesAsync();
                                    }
                                }

                            }
                        }
                    }
                    AccountBal = Convert.ToDecimal(loanacct.AccountBalance);
                }
            }


            return AccountBal;
        }

        protected async void DeductLoanAmountFromLoanAccount(string MemberNumber, int LoanId, int AccountTypeID, decimal Amount, int RepaymentId)
        {
            //var StaffID = context.Staff.Where(a => a.RoleId == 1).ToList().Take(1).SingleOrDefault().StaffID;
            var staff = await context.Staff.Where(a => a.RoleId == 1).FirstOrDefaultAsync();
            var StaffID = staff != null ? staff.StaffID : 0;
            var macct = (from h in context.MemberAccount
                         where h.AccountTypeID == AccountTypeID
                         && h.MemberNumber == MemberNumber
                         select h).ToList().SingleOrDefault();
            if (macct != null)
            {
                var LoanAcct = (from k in context.MemberSavingSummary
                                where k.MemberAccountID == macct.MemberAccountID
                                    && k.MemberNumber == MemberNumber
                                select k).ToList().SingleOrDefault();
                if (LoanAcct != null)
                {
                    decimal AccountBalance = 0, total = Amount;
                    // Check if user account has enough balance
                    if (LoanAcct.AccountBalance > 0 && LoanAcct.AccountBalance >= total)
                    {
                        //Deduct the WithdrawalAmount + Charge DebitAmount from above Account
                        AccountBalance = Convert.ToDecimal(getAccountBalance(MemberNumber, AccountTypeID) - total);

                        //Update SavingSummary Account
                        LoanAcct.AccountBalance = AccountBalance;
                        await context.SaveChangesAsync();

                        DateTime edate = DateTime.Now.Date;

                        var transbal = (from x in context.MemberTransactionBalance
                                        join am in context.MemberAccount on x.MemberAccountID equals am.MemberAccountID
                                        where x.MemberNumber == MemberNumber
                                        && x.MemberAccountID == macct.MemberAccountID && x.TransactionDate == edate
                                        && x.TransactionId == RepaymentId && x.TransactionType == "Debit"
                                        select x).ToList().SingleOrDefault();
                        if (transbal == null)
                        {
                            var repay = (from w in context.LoanRepayment
                                         where w.RepaymentId == RepaymentId
                                         select w).ToList().SingleOrDefault();
                            if (repay != null)
                            {
                                //Generate unique code for every user
                                Guid obja = Guid.NewGuid();

                                MemberTransactionBalance tb = new MemberTransactionBalance();
                                tb.ReferenceUniqueId = Convert.ToString(obja);//Unique Identifier
                                tb.MemberNumber = MemberNumber;
                                tb.Debit = Amount;
                                tb.TransactionDate = DateTime.Now.Date;
                                tb.MemberAccountID = macct.MemberAccountID;
                                tb.TransactionId = RepaymentId;
                                tb.TransactionType = "Debit";
                                tb.Description = "Loan Account Deduction - " + repay.RepaymentDesc;
                                tb.CreatedBy = StaffID;
                                tb.CreatedDate = DateTime.Now.Date;

                                tb.Balance = AccountBalance;
                                await context.MemberTransactionBalance.AddAsync(tb);
                                await context.SaveChangesAsync();


                                tblReceipt rcp = new tblReceipt();
                                rcp.PaymentType = "Loan Repayment Deduction";
                                rcp.PaymentId = RepaymentId;
                                rcp.MemberNumber = MemberNumber;
                                rcp.Description = "Loan Repayment Deduction - " + repay.RepaymentDesc; ;
                                rcp.PaymentStatus = "Payment Confirmed";
                                rcp.ReceiptAmount = Amount;
                                rcp.ReceiptDate = DateTime.Now.Date;
                                rcp.ReceiptStaffID = StaffID;
                                rcp.ReceiptNumber = GetReceiptNo();
                                rcp.TotalAmountPayable = Amount;
                                rcp.CreatedDate = DateTime.Now.Date;
                                rcp.CreatedBy = StaffID;

                                var sal = (from k in context.tblReceipt
                                           where k.PaymentId == rcp.PaymentId && k.PaymentType == rcp.PaymentType
                                           && k.ReceiptDate == rcp.ReceiptDate && k.ReceiptNumber == rcp.ReceiptNumber
                                           select k).ToList().SingleOrDefault();
                                if (sal == null)
                                {
                                    await context.tblReceipt.AddAsync(rcp);
                                    await context.SaveChangesAsync();

                                }
                            }

                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        public string getExpectedTotalAmount(int? LoanId, string MemberNo)
        {
            decimal pAmount = 0;
            string approvedAmt = "0";

            var pAmt = (from i in context.LoanRepaymentMonthlySchedules
                        where i.LoanApplicationId == LoanId && i.MemberNumber == MemberNo
                        select i.MonthRepaymentAmount).ToList().Sum();

            var penAmt = (from i in context.LoanPenalty
                          where i.LoanApplicationId == LoanId && i.MemberNumber == MemberNo
                          select i.PenaltyAmount).ToList().Sum();
            if (pAmt > 0)
            {
                if (penAmt > 0)
                    pAmount = Convert.ToDecimal(pAmt + penAmt);
                else
                    pAmount = Convert.ToDecimal(pAmt);
            }

            return approvedAmt = string.Format("{0:N}", pAmount);
        }
        public string getApprovedAmount(int? LoanId, string MemberNo)
        {
            decimal intAmount = 0;
            string approvedAmt = "0";

            var intAmt = (from i in context.LoanApplication
                          where i.LoanApplicationId == LoanId && i.MemberNumber == MemberNo
                          select i.AmountApproved).ToList().Sum();
            if (intAmt > 0)
            {
                intAmount = Convert.ToDecimal(intAmt);
            }

            return approvedAmt = string.Format("{0:N}", intAmount);
        }

        public string getAppliedAmount(int? LoanId, string MemberNo)
        {
            string appliedAmt = "0";

            var cpt = (from i in context.LoanApplication
                       where i.LoanApplicationId == LoanId && i.MemberNumber == MemberNo
                       select i.PrincipalAmount).ToList().Sum();

            if (cpt > 0)
            {
                appliedAmt = string.Format("{0:N}", cpt);
            }

            return appliedAmt;
        }

        public decimal getTotalAmountRepaid(int? LoanId, string MemberNo)
        {
            decimal Repaid = 0;

            var pAmt = (from i in context.LoanRepayment
                        where i.LoanApplicationId == LoanId && i.MemberNumber == MemberNo
                        select i.RepaymentAmountPaid).ToList().Sum();
            if (pAmt > 0)
            {
                Repaid = Convert.ToDecimal(pAmt);
            }

            return Repaid;
        }

        private string response;
        private string api;
        private string sendto, message, apicommand, subacct, subacctpwd, ownermail, msgtype, senderid;
        //Using SMS247Live SMS API
        protected async void SMSMember(string MemberNo, string MobileNo, string msg)
        {


            var ep = (from u in context.Member
                      where u.MemberNumber == MemberNo
                      select u).ToList().SingleOrDefault();
            if (ep != null)
            {
                string Mobile = null;
                string MemberName = ep.FirstName + " " + ep.LastName;

                Mobile = MobileNo;

                var smsSetting = (from k in context.tblSMSNetworkInfo select k).ToList().Take(1).SingleOrDefault();

                if (smsSetting != null)
                {
                    subacct = smsSetting.Username; // Username
                    subacctpwd = smsSetting.UserPassword; // UserPassword
                    ownermail = smsSetting.APIUrl; // APIUrl
                    msgtype = "0";
                    senderid = smsSetting.ProviderName; // ProviderName
                }
                else
                {
                    //Send payment notification to parent
                    subacct = "FEMISH"; // Username
                    subacctpwd = "Pa56w0rd75"; // UserPassword
                    ownermail = "mcmish33@yahoo.co.uk"; // APIUrl
                    msgtype = "0";
                    senderid = "FEMISH"; // ProviderName
                }

                //Send message to Member
                if (Mobile != null)
                {
                    //Send sms to notify Member
                    sendto = Mobile;

                    apicommand = "http://www.smslive247.com/http/index.aspx?cmd=sendquickmsg&&owneremail=" + ownermail + "&subacct=" + subacct + "&subacctpwd=" + subacctpwd + "&message=" + msg + "&sender=" + senderid + "&sendto=" + sendto + "&msgtype=" + msgtype;

                    WebClient c = new WebClient();
                    string response = c.DownloadString(apicommand);

                    if (response.Contains("ok"))
                    {
                        var org = (from b in context.Organization select b).Take(1).ToList().SingleOrDefault();
                        if (org != null)
                        {
                            var sms = new tblSMSMessage();
                            sms.CreatedDate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                            sms.OrganisationId = org.OrgId;
                            sms.SentDate = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd");
                            sms.SentTime = Convert.ToDateTime(DateTime.Now.Date).ToString("hh:mm");
                            sms.SMSMessage = msg;
                            sms.SMSreceiver = MobileNo;
                            sms.Status = "Sent";
                            sms.CreatedBy = "System Administrator";

                            await context.tblSMSMessage.AddAsync(sms);
                            await context.SaveChangesAsync();
                        }
                    }


                }
            }

        }
        public async void remindermail()
        {
            string constr = configuration.GetConnectionString("SqlConnection");
            SqlConnection con = new SqlConnection(constr);
            DataTable dt = new DataTable();
            //DateTime currDate = DateTime.Now.Date;

            string WebsiteURL = null; String strPathAndQuery = null;
            // strPathAndQuery = "HttpContext.Current.Request.Url.PathAndQuery";
            WebsiteURL = hostingEnvironment.WebRootPath;

            SqlCommand cmd = new SqlCommand(@"Select * FROM LoanRepaymentMonthlySchedules WHERE (PaymentDueDate >=  CAST(GETDATE() as date)  AND DATEDIFF(day, GETDATE(), PaymentDueDate) <= 7) AND (RepaymentMonth = DATENAME(MONTH,GETDATE()) AND RepaymentYear = YEAR(GETDATE())) AND RepaymentStatus = 'Scheduled' ORDER BY PaymentDueDate DESC", con);
            cmd.Connection = con;
            //cmd.Parameters.AddAsyncWithValue("@PaymentDueDate", currDate);

            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                //take query result in datatable
                sda.Fill(dt);
            }

            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j <= dt.Rows.Count - 1; j++)
                {
                    DataRow row1 = dt.Rows[j];
                    decimal apprvLoanAmt = 0;

                    int hr = Convert.ToDateTime(DateTime.Now).Hour;
                    string greetings = string.Empty;

                    if (hr > 11)
                    {
                        if (hr <= 15)//12noon - 3pm
                            greetings = "Enjoy the rest of your day.";

                        else//4pm - 11:59pm
                            greetings = "Have a great evening.";
                    }
                    else//12:00am - 11:59am
                    {
                        if (hr <= 7)//Very early before 8am
                            greetings = "Have a great day.";
                        else
                            //8:00am - 11:00am
                            greetings = "Good day.";
                    }

                    string memberNumber = dt.Rows[j]["MemberNumber"].ToString();  //user studentnumber to use fetch email address in first column
                    decimal monthRepayAmt = Convert.ToDecimal(dt.Rows[j]["MonthRepaymentAmount"]);
                    string rePayStatus = dt.Rows[j]["RepaymentStatus"].ToString();
                    int loanAppId = Convert.ToInt32(dt.Rows[j]["LoanApplicationId"].ToString());//Use LoanAppId to fetch Loan Approved Amount

                    var pp = (from u in context.LoanApplication where u.LoanApplicationId == loanAppId select u).SingleOrDefault();
                    if (pp != null)
                        apprvLoanAmt = Convert.ToDecimal(pp.AmountApproved);

                    DateTime repayDeadlineDate = Convert.ToDateTime(dt.Rows[j]["PaymentDueDate"].ToString()).Date;
                    string reminder = dt.Rows[j]["Reminder"].ToString();
                    int Year = Convert.ToInt32(dt.Rows[j]["RepaymentYear"].ToString());
                    string repayYear = Convert.ToString(Year);
                    string Month = dt.Rows[j]["RepaymentMonth"].ToString();

                    string repaymentMonthName = Convert.ToDateTime(repayDeadlineDate).ToString("MMMM");
                    int rpayMonth = Convert.ToDateTime(repayDeadlineDate).Month;

                    DateTime ddate1 = repayDeadlineDate.AddDays(-7).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate2 = repayDeadlineDate.AddDays(-6).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate3 = repayDeadlineDate.AddDays(-5).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate4 = repayDeadlineDate.AddDays(-4).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate5 = repayDeadlineDate.AddDays(-3).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate6 = repayDeadlineDate.AddDays(-2).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate7 = repayDeadlineDate.AddDays(-1).Date;//Start sending Email from this date to the deadline date.
                    DateTime ddate8 = repayDeadlineDate.Date;//Start sending Email from this date to the deadline date.
                    string email = string.Empty, memberName = string.Empty, subject = string.Empty;

                    if (((repayDeadlineDate >= ddate1 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate1)))
                    {
                        if (string.IsNullOrEmpty(reminder) && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).ToList().SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";

                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                       Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT FIRST REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "First Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    // end of Reminder 1
                    else if ((repayDeadlineDate >= ddate2 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate2))
                    {
                        if (reminder == "First Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).ToList().SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                      Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT SECOND REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Second Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    // end of Reminder 2

                    else if ((repayDeadlineDate >= ddate3 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate3))
                    {
                        if (reminder == "Second Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                      Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT THIRD REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Third Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 3

                    else if ((repayDeadlineDate >= ddate4 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate4))
                    {
                        if (reminder == "Third Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                       Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT FOURTH REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Fourth Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 4

                    else if ((repayDeadlineDate >= ddate5 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate5))
                    {
                        if (reminder == "Fourth Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                      Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT FIFTH REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Fifth Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 5

                    else if ((repayDeadlineDate >= ddate6 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate6))
                    {
                        if (reminder == "Fifth Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                      Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT SIXTH REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Sixth Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 6

                    else if ((repayDeadlineDate >= ddate7 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date == ddate7))
                    {
                        if (reminder == "Sixth Reminder" && rePayStatus == "Scheduled")
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                       Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT SEVENTH REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Seventh Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 7

                    else if (((repayDeadlineDate >= ddate8 && rpayMonth == DateTime.Now.Month && Year == DateTime.Now.Year) && (DateTime.Now.Date >= ddate8)) && (reminder == "Seventh Reminder") && rePayStatus == "Scheduled")
                    {
                        if (((reminder == "Seventh Reminder") && rePayStatus == "Scheduled"))
                        {
                            var st = (from m in context.Member where m.MemberNumber == memberNumber select m).SingleOrDefault();
                            if (!string.IsNullOrEmpty(st.Email))
                            {
                                email = st.Email;
                                memberName = st.FirstName + " " + st.LastName;
                                subject = "QUEENS MPCS - LOAN REPAYMENT REMINDER";
                                string body = this.PopulateBody(WebsiteURL, "Dear " + memberName,
                        Environment.NewLine + Environment.NewLine + "LOAN REPAYMENT EIGHT REMINDER FOR " + repaymentMonthName + " " + Year.ToString(), Environment.NewLine + Environment.NewLine + "This is to bring to your notice that your loan repayment for the above mentioned month is due to be paid on " + Convert.ToDateTime(repayDeadlineDate).ToString("MMMM-dd-yyyy") + ".", " You can make payment any moment from now till the deadline date. " + Environment.NewLine + Environment.NewLine + "You are expected to make a payment of " + string.Format("{0:C}", monthRepayAmt), "Please let us know if you have any question. ", "Thank you for choosing Queens MPCS." + Environment.NewLine + Environment.NewLine + greetings);

                                SendHtmlFormattedEmail(email, subject, body);

                                //Update Reminder Column
                                var ds = (from c in context.LoanRepaymentMonthlySchedules
                                          where c.MemberNumber == st.MemberNumber &&
                                              pp.LoanApplicationId == loanAppId
                                              && c.RepaymentMonth == repaymentMonthName
                                              && c.RepaymentYear == repayYear
                                          select c).ToList().SingleOrDefault();

                                ds.Reminder = "Eight Reminder";
                                await context.SaveChangesAsync();
                            }
                        }
                    }//End of Reminder 8
                }
            }
        }

        private string PopulateBody(string WebsiteURL, string name, string title, string firstP, string secondP, string thirdP, string bestWishes)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(hostingEnvironment.WebRootPath, "EmailPaymentReminder.htm")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{WebsiteURL}", WebsiteURL);
            body = body.Replace("{Name}", name);
            body = body.Replace("{Title}", title);
            body = body.Replace("{FirstParagraph}", firstP);
            body = body.Replace("{SecondParagraph}", secondP);
            body = body.Replace("{ThirdParagraph}", thirdP);
            body = body.Replace("{Bestwishes}", bestWishes);

            return body;
        }

        private void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                var u = (from m in context.NetworkInfo select m).ToList().Take(1).SingleOrDefault();
                string uname, upass, uhost;
                int uport;
                bool ussl = true;
                uname = Convert.ToString(u.NC_UserName);
                upass = Convert.ToString(u.NC_NetworkPassword);
                uhost = Convert.ToString(u.NC_SMTPName);
                uport = Convert.ToInt32(u.NC_PortNumber);
                ussl = Convert.ToBoolean(u.NetworkSSL);


                mailMessage.From = new MailAddress(uname);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recepientEmail));

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = uhost; // "smtp.bizmail.yahoo.com";
                smtp.EnableSsl = ussl; // Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = uname; //  ConfigurationManager.AppSettings["UserName"];
                NetworkCred.Password = upass;  // ConfigurationManager.AppSettings["Password"];
                //smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = uport; // int.Parse(ConfigurationManager.AppSettings["Port"]);
                //smtp.SendAsync(mailMessage, null);
                // smtp.SendCompleted += SentMessageTrigger;
                smtp.Send(mailMessage);
            }
        }

        public string GetLast(string source, int length)
        {
            if (length >= source.Length)
                return source;
            return source.Substring(source.Length - length);
        }
        public string GetReceiptNo()
        {
            string num = string.Empty;

            var xb = context.tblReceipt.Count();
            if (xb > 0)
            {
                var pa = (from k in context.tblReceipt
                          where (k.ReceiptNumber != null && k.ReceiptNumber != "")
                          orderby k.ReceiptID descending
                          select k.ReceiptID).ToList().Max();
                if (pa > 0)
                {
                    int stid = Convert.ToInt32(pa);
                    int id = 1;

                    var us = (from u in context.tblReceipt
                              where u.ReceiptID == stid
                              select u).ToList().SingleOrDefault();
                    if (us != null)
                    {
                        if (us.ReceiptNumber.Contains("QT"))
                        {
                            string SubStringmid = GetLast(us.ReceiptNumber, 5);
                            int submid = (Convert.ToInt32(SubStringmid) + id);

                            if (Convert.ToInt32(submid) <= 9)
                            {
                                num = "QT0000" + submid;
                            }
                            else if (Convert.ToInt32(submid) <= 99)
                            {
                                num = "QT000" + submid;
                            }
                            else if (Convert.ToInt32(submid) <= 999)
                            {
                                num = "QT00" + submid;
                            }
                            else if (Convert.ToInt32(submid) <= 9999)
                            {
                                num = "QT0" + submid;
                            }
                            else if (Convert.ToInt32(submid) <= 99999)
                            {
                                num = "QT" + submid;
                            }
                        }
                        else
                        {
                            num = "QT00001";
                        }
                    }
                    else
                    {
                        num = "QT00001";
                    }
                }
            }
            else
            {
                num = "QT00001";
            }
            return num;
        }
        public async Task<StatusResponse> AuthenticateUser(string username, string passwrd)
        {
            StatusResponse response = new();

            String user = username.Replace("'", "''");
            String password = passwrd.Replace("'", "''");
            var session = new tblSessionLog();

            try
            {
                if (user.StartsWith("0"))
                {
                    string mobile = user.Substring(1, user.Length);
                    user = "234" + mobile;
                }
                var au = await context.Member.Where(x => (x.MemberNumber == user || x.Mobile == user) && x.RoleId == 3 && x.MemberStatus == "Active").Select(c => new
                {
                    RoleId = c.RoleId,
                    Password = c.Password,
                    MemberId = c.MemberId,
                    MemberNumber = c.MemberNumber
                }).FirstOrDefaultAsync(); //Member


                if (au != null)
                {
                    string ps = au.Password;

                    if (ps != "")
                    {
                        string hsvalue = ps;
                        bool flag = HshClass.VerifyHash(password, "SHA512", hsvalue);

                        if (flag == true)
                        {
                            if (au.RoleId == 3)//Member
                            {
                                try
                                {
                                    if (await context.LoanRepaymentMonthlySchedules.AnyAsync(x => x.RepaymentStatus == "Unpaid" || x.RepaymentStatus == "Overdue Payment"))
                                    { CallAutoLoanRePaymentDebit(); }
                                }
                                catch (Exception ex)
                                {
                                    //
                                }

                                try
                                {
                                    if (await context.LoanRepaymentMonthlySchedules.AnyAsync(x => x.RepaymentStatus == "Unpaid" || x.RepaymentStatus == "Overdue Payment"))
                                    { CallAutoFlagOverDuePaymentProcedure(); }
                                }
                                catch (Exception ex)
                                {
                                    //
                                }

                                try
                                {
                                    remindermail();
                                }
                                catch (Exception ex)
                                {
                                    //
                                }
                                try
                                {
                                    CallAutoCreditInterestOnSavingsProcedure();
                                }
                                catch (Exception ex)
                                {
                                    //
                                }


                                ///Call Audit Trail function to insert into audit Trail table
                                uac.insertAtrail("Member", "Login", au.MemberId, "MemberNumber,Password", null, au.MemberNumber + "," + au.Password);

                                ///Capture user session log info ans insert into SessionLog table
                                string stHostName = "";
                                stHostName = System.Net.Dns.GetHostName();
                                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(stHostName);
                                IPAddress[] addr = ipEntry.AddressList;
                                //ipLabel.Text = addr[addr.Length - 2].ToString();
                                session.HostAddress = addr[addr.Length - 2].ToString();
                                session.HostName = stHostName; // Page.Request.UserHostName;
                                                               //session.UserAgent = Page.Request.UserAgent;
                                session.StaffID = au.MemberId;
                                session.SessionStart = Convert.ToString(DateTime.UtcNow.ToShortTimeString());
                                var last = (from m in context.tblSessionLog
                                            where m.MemberNumber == au.MemberNumber
                                            orderby m.SessionLogID descending
                                            select m).ToList().Take(1).SingleOrDefault();
                                if (last != null)
                                {
                                    session.LastActivity = last.LastActivity;
                                }
                                await context.tblSessionLog.AddAsync(session);
                                await context.SaveChangesAsync();

                                response.Result = $"Member:{3}";
                                response.Status = StatusCodes.Status200OK;


                            }
                        }
                        else
                        {
                            response.Result = "Invalid  password. Please enter valid  password.";
                            response.Status = StatusCodes.Status400BadRequest;
                        }
                    }
                }
                else
                {
                    var stf = await context.Staff.Where(x => x.StaffNumber == user && x.StaffStatus == "Active").Select(c => new
                    {
                        RoleId = c.RoleId,
                        Password = c.Password,
                        StaffID = c.StaffID,
                        StaffNumber = c.StaffNumber
                    }).FirstOrDefaultAsync();

                    if (stf != null)
                    {
                        string hsvalue = stf.Password;
                        bool flag = HshClass.VerifyHash(password, "SHA512", hsvalue);

                        if (flag == true)
                        {
                            //var exists =  context.LoanRepaymentMonthlySchedules
                            //                .Any(x => x.RepaymentStatus == "Unpaid" || x.RepaymentStatus == "Overdue Payment");
                            try
                            {


                                //if (exists == true)
                            //    CallAutoLoanRePaymentDebit();
                            }
                            catch (Exception ex)
                            {
                                //
                            }
                            try
                            {
                                //if (exists == true)
                             //   CallAutoFlagOverDuePaymentProcedure();
                            }
                            catch (Exception ex)
                            {
                                //
                            }

                            try
                            {
                              //  remindermail();
                            }
                            catch (Exception ex)
                            {
                                //
                            }
                            try
                            {
                              //  CallAutoCreditInterestOnSavingsProcedure();
                            }
                            catch (Exception ex)
                            {
                                //
                            }
                            if (stf.RoleId == 1)//Admin
                            {
/*
                                try
                                {
                                    ///Call Audit Trail function to insert into audit Trail table
                                    uac.insertAtrail("Staff", "Login", stf.StaffID, "StaffNumber,Password", null, stf.StaffNumber + "," + stf.Password);

                                    ///Capture user session log info ans insert into SessionLog table
                                    string stHostName = "";
                                    stHostName = System.Net.Dns.GetHostName();
                                    IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(stHostName);
                                    IPAddress[] addr = ipEntry.AddressList;
                                    //ipLabel.Text = addr[addr.Length - 2].ToString();
                                    session.HostAddress = addr[addr.Length - 2].ToString();
                                    session.HostName = stHostName; // Page.Request.UserHostName;
                                    //session.UserAgent = Page.Request.UserAgent;
                                    session.StaffID = stf.StaffID;
                                    session.SessionStart = Convert.ToString(DateTime.UtcNow.ToShortTimeString());
                                    var last = (from m in context.tblSessionLog
                                                where m.StaffID == stf.StaffID
                                                orderby m.SessionLogID descending
                                                select m).ToList().Take(1).SingleOrDefault();
                                    if (last != null)
                                    {
                                        session.LastActivity = last.LastActivity;
                                    }
                                    else
                                    {
                                        session.LastActivity = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd HH:mm");
                                    }
                                    await context.tblSessionLog.AddAsync(session);
                                    await context.SaveChangesAsync();

                                }
                                catch (Exception c)
                                {
                                    //
                                }
                                */
                                response.Result = $"Admin:{1}";
                                response.Status = StatusCodes.Status200OK;
                            }
                            else if (stf.RoleId == 2)//Staff
                            {
                                /*
                                uac.insertAtrail("Staff", "Login", stf.StaffID, "StaffNumber,Password", null, stf.StaffNumber + "," + stf.Password);

                                ///Capture user session log info ans insert into SessionLog table
                                string stHostName = "";
                                stHostName = System.Net.Dns.GetHostName();
                                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(stHostName);
                                IPAddress[] addr = ipEntry.AddressList;
                                //ipLabel.Text = addr[addr.Length - 2].ToString();
                                session.HostAddress = addr[addr.Length - 2].ToString();
                                session.HostName = stHostName; // Page.Request.UserHostName;
                                                               // session.UserAgent = Page.Request.UserAgent;
                                session.StaffID = stf.StaffID;
                                session.SessionStart = Convert.ToString(DateTime.UtcNow.ToShortTimeString());
                                var last = (from m in context.tblSessionLog
                                            where m.StaffID == stf.StaffID
                                            orderby m.SessionLogID descending
                                            select m).ToList().Take(1).SingleOrDefault();
                                if (last != null)
                                {
                                    session.LastActivity = last.LastActivity;
                                }
                                else
                                {
                                    session.LastActivity = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd HH:mm");
                                }
                                await context.tblSessionLog.AddAsync(session);
                                await context.SaveChangesAsync();

                                */
                                response.Result = $"Staff:{2}";
                                response.Status = StatusCodes.Status200OK;
                            }
                            else if (stf.RoleId == 4)//Staff
                            {
                                /*
                                ///Call Audit Trail function to insert into audit Trail table
                                uac.insertAtrail("Staff", "Login", stf.StaffID, "StaffNumber,Password", null, stf.StaffNumber + "," + stf.Password);

                                ///Capture user session log info ans insert into SessionLog table
                                string stHostName = "";
                                stHostName = System.Net.Dns.GetHostName();
                                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(stHostName);
                                IPAddress[] addr = ipEntry.AddressList;
                                //ipLabel.Text = addr[addr.Length - 2].ToString();
                                session.HostAddress = addr[addr.Length - 2].ToString();
                                session.HostName = stHostName; // Page.Request.UserHostName;
                                                               //  session.UserAgent = Page.Request.UserAgent;
                                session.StaffID = stf.StaffID;
                                session.SessionStart = Convert.ToString(DateTime.UtcNow.ToShortTimeString());

                                var last = (from m in context.tblSessionLog
                                            where m.StaffID == stf.StaffID
                                            orderby m.SessionLogID descending
                                            select m).ToList().Take(1).SingleOrDefault();
                                if (last != null)
                                {
                                    session.LastActivity = last.LastActivity;
                                }
                                else
                                {
                                    session.LastActivity = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyy-MM-dd HH:mm");
                                }

                                await context.tblSessionLog.AddAsync(session);
                                await context.SaveChangesAsync();

                                */

                                response.Result = $"Staff:{4}";
                                response.Status = StatusCodes.Status200OK;
                            }

                        }
                        else
                        {
                            response.Result = "Invalid  password. Please enter valid  password.";
                            response.Status = StatusCodes.Status400BadRequest;
                        }
                    }
                    else
                    {
                        response.Result = "Account is either inactive or does not exists.";
                        response.Status = StatusCodes.Status404NotFound;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<StatusResponse> Register(Member member, Staff staff)
        {
            var response = new StatusResponse();
            try
            {
                if (member is not null)
                {
                    if (member.MemberId is 0)
                    {
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
                else
                {
                    if (staff.StaffID is 0)
                    {
                        await context.Staff.AddAsync(staff);
                        response.Status = 200;
                        response.Result = "Staff successfully created";
                    }
                    else
                    {
                        context.Staff.Update(staff);
                        response.Status = 200;
                        response.Result = "Staff record successfully updated";
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                response.Status = 400;
                response.Result = $"Sorry, an error occured please try again {ex.Message}";
            }

            return response;
        }
    }
}
