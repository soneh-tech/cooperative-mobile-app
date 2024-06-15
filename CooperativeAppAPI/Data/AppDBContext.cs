namespace CooperativeAppAPI.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<audittrail> audittrail { get; set; }
        public virtual DbSet<audittrailHistory> audittrailHistory { get; set; }
        public virtual DbSet<BillingType> BillingType { get; set; }
        public virtual DbSet<BirthDayCerebration> BirthDayCerebration { get; set; }
        public virtual DbSet<CompanyAccountDetail> CompanyAccountDetail { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<ContributionType> ContributionType { get; set; }
        public virtual DbSet<CoorperatorBooklet> CoorperatorBooklet { get; set; }
        public virtual DbSet<DeductionType> DeductionType { get; set; }
        public virtual DbSet<Delegation> Delegation { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<EmailMessage> EmailMessage { get; set; }
        public virtual DbSet<LeadPosition> LeadPosition { get; set; }
        public virtual DbSet<LoanApplication> LoanApplication { get; set; }
        public virtual DbSet<LoanRepaymentPlan> LoanRepaymentPlan { get; set; }
        public virtual DbSet<LoanRepaymentPlanType> LoanRepaymentPlanType { get; set; }
        public virtual DbSet<LoanRepaymentSummary> LoanRepaymentSummary { get; set; }
        public virtual DbSet<LoanType> LoanType { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<MemberAccount> MemberAccount { get; set; }
        public virtual DbSet<MemberBank> MemberBank { get; set; }
        public virtual DbSet<MemberGuarantor> MemberGuarantor { get; set; }
        public virtual DbSet<MemberSavingDefinition> MemberSavingDefinition { get; set; }
        public virtual DbSet<MemberSavingSummary> MemberSavingSummary { get; set; }
        public virtual DbSet<MemberTransactionBalance> MemberTransactionBalance { get; set; }
        public virtual DbSet<MemberWithdrawal> MemberWithdrawal { get; set; }
        public virtual DbSet<NetworkInfo> NetworkInfo { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<RegistrationFee> RegistrationFee { get; set; }
        public virtual DbSet<ResetPass> ResetPass { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Staff_Sal> Staff_Sal { get; set; }
        public virtual DbSet<StaffCategory> StaffCategory { get; set; }
        public virtual DbSet<StaffPension> StaffPension { get; set; }
        public virtual DbSet<StaffSalaryDefinition> StaffSalaryDefinition { get; set; }
        public virtual DbSet<StaffTax> StaffTax { get; set; }
        public virtual DbSet<SupportTicketRepons> SupportTicketReponses { get; set; }
        public virtual DbSet<SupportTickett> SupportTicketts { get; set; }
        public virtual DbSet<tblAdministrativeCharge> tblAdministrativeCharge { get; set; }
        public virtual DbSet<tblAsset> tblAsset { get; set; }
        public virtual DbSet<tblaudittrail> tblaudittrail { get; set; }
        public virtual DbSet<tblAuditTrailMod> tblAuditTrailMod { get; set; }
        public virtual DbSet<tblBank> tblBank { get; set; }
        public virtual DbSet<tblExpens> tblExpenses { get; set; }
        public virtual DbSet<tblExpensesCategory> tblExpensesCategory { get; set; }
        public virtual DbSet<tblExpensesType> tblExpensesType { get; set; }
        public virtual DbSet<tblGeneralJournal> tblGeneralJournal { get; set; }
        public virtual DbSet<tblInterestOnAccount> tblInterestOnAccount { get; set; }
        public virtual DbSet<tblLedger> tblLedger { get; set; }
        public virtual DbSet<tblLGA> tblLGA { get; set; }
        public virtual DbSet<tblNotificationMethod> tblNotificationMethod { get; set; }
        public virtual DbSet<tblOTPHistory> tblOTPHistory { get; set; }
        public virtual DbSet<tblReceipt> tblReceipt { get; set; }
        public virtual DbSet<tblSessionLog> tblSessionLog { get; set; }
        public virtual DbSet<tblShareHoldersEquity> tblShareHoldersEquity { get; set; }
        public virtual DbSet<tblSMSMessage> tblSMSMessage { get; set; }
        public virtual DbSet<tblSMSNetworkInfo> tblSMSNetworkInfo { get; set; }
        public virtual DbSet<tblState> tblState { get; set; }
        public virtual DbSet<tempLoanCalculationtable> tempLoanCalculationtable { get; set; }
        public virtual DbSet<temptblBalanceSheet> temptblBalanceSheet { get; set; }
        public virtual DbSet<temptblIncomeandExpenditure> temptblIncomeandExpenditures { get; set; }
        public virtual DbSet<UserDelegation> UserDelegation { get; set; }
        public virtual DbSet<UserReferralTracking> UserReferralTracking { get; set; }
        public virtual DbSet<WFStatu> WFStatus { get; set; }
        public virtual DbSet<LoanRepaymentMonthlySchedule> LoanRepaymentMonthlySchedules { get; set; }
        public virtual DbSet<LoanRepayment> LoanRepayment { get; set; }
        public virtual DbSet<LoanPenalty> LoanPenalty { get; set; }
        public virtual DbSet<MemberSaving_Deposit> MemberSaving_Deposit { get; set; }
    }
}
