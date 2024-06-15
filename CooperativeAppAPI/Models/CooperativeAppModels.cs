using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CooperativeAppAPI.Models
{
    public class MemberSaving_Deposit
    {
        [Key]
        public int MemberSavingId { get; set; }
        public int? MemberSavingDefinitionId { get; set; }
        public int? MemberAccountID { get; set; }
        public int? MemberId { get; set; }
        public string? MemberNumber { get; set; }
        public int? AccountTypeId { get; set; }
        public decimal? Amount { get; set; }
        public int? SavingDay { get; set; }
        public DateTime? SavingDate { get; set; }
        public int? SavingMonth { get; set; }
        public int? SavingYear { get; set; }
        public decimal? AmountDeposited { get; set; }
        public string? ConfirmationStatus { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? CreatedByUserRole { get; set; }
        public DateTime? HODDateReviewed { get; set; }
        public int? HODReviewedBy { get; set; }
        public string? HODReviewComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ApprovalStatus { get; set; }
        public string? DepositTime { get; set; }

        public AccountType? AccountType { get; set; }
        public Member? Member { get; set; }
        public MemberAccount? MemberAccount { get; set; }
        public MemberSavingDefinition? MemberSavingDefinition { get; set; }
    }
    public class LoanPenalty
    {
        [Key]
        public int LoanPenaltyId { get; set; }
        public int? LoanApplicationId { get; set; }
        public string? MemberNumber { get; set; }
        public string? RepaymentMonth { get; set; }
        public string? RepaymentYear { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public decimal? TotalBalance { get; set; }
    }

    public class LoanRepayment
    {
        [Key]
        public int RepaymentId { get; set; }
        public string? RepaymentDesc { get; set; }
        public int? RepaymentPlanId { get; set; }
        public decimal? RepaymentAmount { get; set; }
        public decimal? RepaymentAmountPaid { get; set; }
        public decimal? OutstandingBal { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public DateTime? RepaymentDueDate { get; set; }
        public int? AccountTypeId { get; set; }
        public string? DatePaid { get; set; }
        public int? AnytPenalty { get; set; }
        public string? MemberNumber { get; set; }
        public int? RepaymentCount { get; set; }
        public int? LoanApplicationId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? HODDateReviewed { get; set; }
        public int? HODReviewedBy { get; set; }
        public string? HODReviewComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ApprovalStatus { get; set; }

        public LoanApplication? LoanApplication { get; set; }
        public Member? Member { get; set; }
        public LoanRepaymentMonthlySchedule? LoanRepaymentMonthlySchedule { get; set; }
    }

    public class LoanRepaymentMonthlySchedule
    {
        [Key]
        public int RepaymentPlanId { get; set; }
        public int? LoanApplicationId { get; set; }
        public int? AccountTypeId { get; set; }
        public int? RepaymentPlanTypeId { get; set; }
        public string? MemberNumber { get; set; }
        public decimal? RemainingPrincipalBal { get; set; }
        public decimal? MonthRepaymentAmount { get; set; }
        public decimal? MonthlyPrincipalBal { get; set; }
        public decimal? MonthlyInterestAmount { get; set; }
        public decimal? PenaltyAmount { get; set; }
        public int? RepaymentScheduleNumber { get; set; }
        public string? RepaymentYear { get; set; }
        public string? RepaymentMonth { get; set; }
        public string? RepaymentDay { get; set; }
        public int? NumberofMonths { get; set; }
        public string? RepaymentStatus { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? Reminder { get; set; }

        public LoanApplication? LoanApplication { get; set; }
        public LoanRepaymentPlanType? LoanRepaymentPlanType { get; set; }
        public Member? Member { get; set; }
        public IEnumerable<LoanRepayment>? LoanRepayments { get; set; }
    }

    public class WFStatu
    {
        [Key]
        public int WFStatusID { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class UserReferralTracking
    {
        [Key]
        public int Id { get; set; }
        public string? MemberNumber { get; set; }
        public string? ReferralCode { get; set; }
        public string? EmailAddress { get; set; }
        public string? MyReferralStatus { get; set; }
    }

    public class UserDelegation
    {
        [Key]
        public int DelegationID { get; set; }
        public int? UserProfileId { get; set; }
        public string? Description { get; set; }
        public string? EmployeeNumber { get; set; }
        public int? DelegatedBy { get; set; }
        public int? DelegationDept { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class temptblIncomeandExpenditure
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CHARTOFACCOUNT { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public int? TransactionMonth { get; set; }
        public int? TransactionYear { get; set; }
        public string? Type { get; set; }
    }

    public class temptblBalanceSheet
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public int? AccountingCode { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public int? TransactionMonth { get; set; }
        public int? TransactionYear { get; set; }
        public string? Type { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class tempLoanCalculationtable
    {
        [Key]
        public int Id { get; set; }
        public decimal? OriginalPrincipalBal { get; set; }
        public decimal? RemainingPrincipalBal { get; set; }
        public decimal? OverallTotalPayable { get; set; }
        public decimal? PercentageInterest { get; set; }
        public int? ScheduleNumber { get; set; }
        public string? MemberNumber { get; set; }
        public int? LoanApplicationId { get; set; }
        public int? ReOrderLevel { get; set; }
    }

    public class tblState
    {
        [Key]
        public int StateId { get; set; }
        public string? StateDescription { get; set; }
        public string? StateCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public IEnumerable<tblLGA>? tblLGAs { get; set; }
    }

    public class tblSMSNetworkInfo
    {
        [Key]
        public int SMSNetworkInoID { get; set; }
        public string? HttpAPIKey { get; set; }
        public string? RestApiKey { get; set; }
        public string? Username { get; set; }
        public string? UserPassword { get; set; }
        public string? APIUrl { get; set; }
        public string? ProviderName { get; set; }
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class tblSMSMessage
    {
        [Key]
        public int SMSMessageID { get; set; }
        public string? SMSMessage { get; set; }
        public string? SentDate { get; set; }
        public string? SentTime { get; set; }
        public string? SMSreceiver { get; set; }
        public string? Status { get; set; }
        public string? SMSMessageCode { get; set; }
        public int? TotalSuccess { get; set; }
        public int? TotalFailure { get; set; }
        public int? TotalCharged { get; set; }
        public decimal? CurrentBalance { get; set; }
        public int? OrganisationId { get; set; }
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class tblShareHoldersEquity
    {
        [Key]
        public int ShareHoldersEquityId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? InvestmentCapital { get; set; }
        public decimal? PercentageShare { get; set; }
        public decimal? RetainedEarnings { get; set; }
        public DateTime? InvestmentDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class tblSessionLog
    {
        [Key]
        public int SessionLogID { get; set; }
        public int? StaffID { get; set; }
        public string? MemberNumber { get; set; }
        public string? Type { get; set; }
        public string? UserAgent { get; set; }
        public string? HostAddress { get; set; }
        public string? HostName { get; set; }
        public string? LastActivity { get; set; }
        public int? SessionID { get; set; }
        public string? SessionStart { get; set; }
        public string? SessionEnd { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public Staff? Staff { get; set; }
    }

    public class tblReceipt
    {
        [Key]
        public int ReceiptID { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? Description { get; set; }
        public string? MemberNumber { get; set; }
        public decimal? TotalAmountPayable { get; set; }
        public decimal? ReceiptAmount { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public int? PaymentId { get; set; }
        public string? PaymentType { get; set; }
        public int? ReceiptStaffID { get; set; }
        public string? PaymentStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? PayProof { get; set; }
        public string? ModeOfPayment { get; set; }
        public string? BankName { get; set; }
        public string? TellerNumber { get; set; }
        public string? PaymentRefCode { get; set; }
        public string? Comment { get; set; }
        public int? OrgId { get; set; }

        public Member? Member { get; set; }
    }

    public class tblOTPHistory
    {
        [Key]
        public int OTPHistoryId { get; set; }
        public string? ctime_Stamp { get; set; }
        public string? OTPCode { get; set; }
        public string? Status { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? MemberNumber { get; set; }
        public int? TansactionRecordId { get; set; }
        public string? PaymentType { get; set; }
    }

    public class tblNotificationMethod
    {
        [Key]
        public int NotificationMethodID { get; set; }
        public string? NotificationMethodDesc { get; set; }
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDate { get; set; }
    }

    public class tblLGA
    {
        [Key]
        public int LGAId { get; set; }
        public string? LGAName { get; set; }
        public string? LGACode { get; set; }
        public int? StateId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }

        public tblState? tblState { get; set; }
    }

    public class tblLedger
    {
        [Key]
        public int LedgerID { get; set; }
        public int? ID { get; set; }
        public string? LedgerType { get; set; }
        public DateTime? LDate { get; set; }
        public string? LDebitDesc { get; set; }
        public decimal? LDebitAmount { get; set; }
        public decimal? LDebitBalBD { get; set; }
        public string? LCreditDesc { get; set; }
        public decimal? LCreditAmount { get; set; }
        public decimal? LCreditBalBD { get; set; }
        public string? Status { get; set; }
        public int? OrganizationId { get; set; }
    }

    public class tblInterestOnAccount
    {
        [Key]
        public int InterestOnAccountId { get; set; }
        public string? CreditDescription { get; set; }
        public decimal? CreditAmount { get; set; }
        public DateTime? CreditDate { get; set; }
        public string? MemberNumber { get; set; }
        public int? MemberAccountID { get; set; }
        public int? AccountTypeId { get; set; }
        public string? CreditStatus { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }

    public class tblGeneralJournal
    {
        [Key]
        public int GeneralJournalID { get; set; }
        public string? JournalType { get; set; }
        public DateTime? JournalDate { get; set; }
        public string? DebitDesc { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal? DebitBalBD { get; set; }
        public string? CreditDesc { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? CreditBalBD { get; set; }
        public string? Status { get; set; }
        public int? StaffID { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? ChartAccount { get; set; }
        public int? TransactionYear { get; set; }
        public int? TransactionMonth { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class tblExpensesType
    {
        [Key]
        public int ExpensesTypeID { get; set; }
        public string? ExpensesTypeDesc { get; set; }
        public int? ExpensesCategoryID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StaffID { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<tblExpens>? tblExpenses { get; set; }
    }

    public class tblExpensesCategory
    {
        [Key]
        public int ExpensesCategoryID { get; set; }
        public string? ExpensesCategoryDesc { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StaffID { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<tblExpens>? tblExpenses { get; set; }
    }

    public class tblExpens
    {
        [Key]
        public int ExpensesId { get; set; }
        public string? Description { get; set; }
        public int? ExpensesCategoryID { get; set; }
        public int? ExpensesTypeID { get; set; }
        public DateTime? ExpensesDate { get; set; }
        public decimal? Amount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? ExpensesByWho { get; set; }
        public string? Reason { get; set; }
        public int? StaffID { get; set; }
        public int? TransactionYear { get; set; }
        public int? TransactionMonth { get; set; }
        public string? ModeOfPayment { get; set; }
        public string? Bank { get; set; }
        public string? AccruedPrepaid { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public Staff? Staff { get; set; }
        public tblExpensesCategory? tblExpensesCategory { get; set; }
        public tblExpensesType? tblExpensesType { get; set; }
    }

    public class tblBank
    {
        public string? BankName { get; set; }
        public string? Slug { get; set; }
        public string? Code { get; set; }
        public string? ussd { get; set; }
        public string? Logo { get; set; }
        [Key]
        public int BankId { get; set; }
    }

    public class tblAuditTrailMod
    {
        [Key]
        public int AuditTrailModId { get; set; }
        public int? tableId { get; set; }
        public string? OriginalValue { get; set; }
        public string? ModifiedValue { get; set; }
        public string? ModifiedFields { get; set; }
        public string? tablename { get; set; }
        public string? operation { get; set; }
        public string? occurreddate { get; set; }
    }

    public class tblaudittrail
    {
        [Key]
        public int audittrailid { get; set; }
        public string? tablename { get; set; }
        public string? operation { get; set; }
        public string? occurreddate { get; set; }
        public string? timeoccurred { get; set; }
        public string? performedbyname { get; set; }
        public int? performedbyid { get; set; }
        public string? fieldname { get; set; }
        public string? oldvalue { get; set; }
        public string? newvalue { get; set; }
        public int? tableId { get; set; }
        public string? OriginalValue { get; set; }
        public string? ModifiedValue { get; set; }
        public string? ModifiedFields { get; set; }
    }

    public class tblAsset
    {
        [Key]
        public int AssetID { get; set; }
        public string? AssetName { get; set; }
        public int? StaffID { get; set; }
        public DateTime? PurchasedDate { get; set; }
        public decimal? AmountPurchased { get; set; }
        public decimal? DepreciationPercentage { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public DateTime? NextServiceDate { get; set; }
        public int? PurchasedBy { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? ReceiptSampleCopy { get; set; }
        public string? AssetImage { get; set; }
        public decimal? CurrentValue { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? Remark { get; set; }
        public string? AssetType { get; set; }
        public string? Status { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public Staff? Staff { get; set; }
    }

    public class tblAdministrativeCharge
    {
        [Key]
        public int AdministrativeChargeId { get; set; }
        public string? ChargeDescription { get; set; }
        public decimal? DebitAmount { get; set; }
        public DateTime? ChargeDate { get; set; }
        public int? ChargeTableId { get; set; }
        public string? ChargeType { get; set; }
        public string? MemberNumber { get; set; }
        public string? ChargeStatus { get; set; }
        public int? MemberAccountID { get; set; }
        public int? AccountTypeId { get; set; }
    }

    public class SupportTickett
    {
        [Key]
        public int TicketId { get; set; }
        public string? TicketNo { get; set; }
        public DateTime? TicketDate { get; set; }
        public string? UserName { get; set; }
        public string? TicketTitle { get; set; }
        public string? Department { get; set; }
        public string? RelatedPlan { get; set; }
        public string? TicketPriority { get; set; }
        public string? Message { get; set; }
        public string? TicketStatus { get; set; }
        public string? LastUpdated { get; set; }
        public string? AttachmentURL { get; set; }
        public string? StaffUserName { get; set; }
        public string? EmailSentStatus { get; set; }
        public string? UserType { get; set; }

        public IEnumerable<SupportTicketRepons>? SupportTicketReponses { get; set; }
    }

    public class SupportTicketRepons
    {
        [Key]
        public int TicketReponseId { get; set; }
        public string? TicketNo { get; set; }
        public DateTime? TicketDate { get; set; }
        public string? UserName { get; set; }
        public string? TicketTitle { get; set; }
        public string? Department { get; set; }
        public string? RelatedPlan { get; set; }
        public string? TicketPriority { get; set; }
        public string? Message { get; set; }
        public string? TicketStatus { get; set; }
        public string? LastUpdated { get; set; }
        public string? AttachmentURL { get; set; }
        public string? StaffUserName { get; set; }
        public string? EmailSentStatus { get; set; }
        public string? UserType { get; set; }
        public int? TicketId { get; set; }

        public SupportTickett? SupportTickett { get; set; }
    }

    public class StaffTax
    {
        [Key]
        public int TaxId { get; set; }
        public string? Description { get; set; }
        public decimal? TaxRate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class StaffSalaryDefinition
    {
        [Key]
        public int StaffSalaryDefinitionId { get; set; }
        public int? StaffID { get; set; }
        public decimal? BeginGrossSalary { get; set; }
        public decimal? BeginNetSalary { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? HomeAllowance { get; set; }
        public decimal? TransportAllowance { get; set; }
        public decimal? UtilityAllowance { get; set; }
        public decimal? CurrentGrosstSalary { get; set; }
        public decimal? CurrentNetSalary { get; set; }
        public DateTime? CurrentSalaryBeginDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? remarks { get; set; }
        public int? headDeptId { get; set; }
        public string? headDeptremarks { get; set; }
        public DateTime? headDeptdatemodified { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public DateTime? LastDateIncreaseSalary { get; set; }

        public Staff? Staff { get; set; }
    }

    public class StaffPension
    {
        [Key]
        public int PensionId { get; set; }
        public string? Description { get; set; }
        public decimal? PensionEmployeeRate { get; set; }
        public decimal? PensionEmployerRate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class StaffCategory
    {
        [Key]
        public int StaffCatID { get; set; }
        public string? CatDescription { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class Staff_Sal
    {
        [Key]
        public int StaffSalaryId { get; set; }
        public int? StaffID { get; set; }
        public decimal? BeginGrossSalary { get; set; }
        public decimal? BeginNetSalary { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? HomeAllowance { get; set; }
        public decimal? TransportAllowance { get; set; }
        public decimal? UtilityAllowance { get; set; }
        public decimal? CurrentGrosstSalary { get; set; }
        public decimal? PensionAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? CurrentNetSalary { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? remarks { get; set; }
        public int? headDeptId { get; set; }
        public string? headDeptremarks { get; set; }
        public DateTime? headDeptdatemodified { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
        public DateTime? CurrentSalaryBeginDate { get; set; }
        public DateTime? LastDateIncreaseSalary { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public Staff? Staff { get; set; }
    }

    public class Staff
    {
        [Key]
        public int StaffID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber1 { get; set; }
        public string? MobileNumber2 { get; set; }
        public int? LocationID { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? StaffCatID { get; set; }
        public int? CreatedBy { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? StaffStatus { get; set; }
        public string? Password { get; set; }
        public string? PersonalEmail { get; set; }
        public string? City { get; set; }
        public string? StateOfOrigin { get; set; }
        public string? Nationality { get; set; }
        public string? HomeTel { get; set; }
        public int? RoleId { get; set; }
        public string? RoleDesc { get; set; }
        public string? StaffPosition { get; set; }
        public string? StaffNumber { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<AccountType>? AccountTypes { get; set; }
        public IEnumerable<BirthDayCerebration>? BirthDayCerebrations { get; set; }
        public IEnumerable<LoanApplication>? LoanApplications { get; set; }
        public IEnumerable<Member>? Members { get; set; }
        public IEnumerable<ResetPass>? ResetPasses { get; set; }
        public Role? Role { get; set; }
        public IEnumerable<Staff_Sal>? Staff_Sal { get; set; }
        public IEnumerable<StaffSalaryDefinition>? StaffSalaryDefinitions { get; set; }
        public IEnumerable<tblAsset>? tblAssets { get; set; }
        public IEnumerable<tblExpens>? tblExpenses { get; set; }
        public IEnumerable<tblSessionLog>? tblSessionLogs { get; set; }
    }

    public class Role
    {
        [Key]
        public int Roleid { get; set; }
        public string? Rolename { get; set; }
        public DateTime? Createddate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<Member>? Members { get; set; }
        public IEnumerable<Staff>? Staffs { get; set; }
    }

    public class ResetPass
    {
        [Key]
        public int PasswordChangeID { get; set; }
        public int? StaffID { get; set; }
        public int? PassChangeStaffID { get; set; }
        public string? PassChangeReason { get; set; }
        public string? ResetCode { get; set; }
        public string? HRcomment { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string? CreatedBy { get; set; }

        public Staff? Staff { get; set; }
    }

    public class RegistrationFee
    {
        [Key]
        public int RegistrationFeeId { get; set; }
        public decimal? RegistrationFee1 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
    }

    public class Organization
    {
        [Key]
        public int OrgId { get; set; }
        public string? OrgName { get; set; }
        public DateTime? IncoperationDate { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? Logo { get; set; }
        public string? LogoContentType { get; set; }
        public string? WebsiteURL { get; set; }
        public string? Motto { get; set; }
        public string? OrganizationInitials { get; set; }
        public string? ZoomEmail { get; set; }
    }

    public class NetworkInfo
    {
        [Key]
        public int NC_PK { get; set; }
        public string? NC_SMTPName { get; set; }
        public string? NC_UserName { get; set; }
        public string? NC_NetworkPassword { get; set; }
        public int? NC_PortNumber { get; set; }
        public bool? NetworkSSL { get; set; }
        public int? NC_CUST_FK { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
    }

    public class MemberWithdrawal
    {
        [Key]
        public int MemberWithdrawalID { get; set; }
        public string? WithdrawalDesc { get; set; }
        public int? MemberId { get; set; }
        public string? MemberNumber { get; set; }
        public int? MemberAccountID { get; set; }
        public decimal? WithdrawalAmount { get; set; }
        public DateTime? WithdrawalRequestedDate { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public string? WithdrawalStatus { get; set; }
        public decimal? AdminFeeCharge { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByUserRole { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? WithdrawalType { get; set; }
        public string? DebitStatus { get; set; }
        public string? DebitedTime { get; set; }
        public DateTime? HODDateReviewed { get; set; }
        public int? HODReviewedBy { get; set; }
        public string? HODReviewComment { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ApprovalStatus { get; set; }

        public Member? Member { get; set; }
        public MemberAccount? MemberAccount { get; set; }
    }

    public class MemberTransactionBalance
    {
        [Key]
        public int MemberTransactionBalanceId { get; set; }
        public string? Description { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Balance { get; set; }
        public int? TransactionId { get; set; }
        public string? TransactionType { get; set; }
        public string? MemberNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? MemberAccountID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public Member? Member { get; set; }
        public MemberAccount? MemberAccount { get; set; }
    }

    public class MemberSavingSummary
    {
        [Key]
        public int SavingSummaryId { get; set; }
        public decimal? TotalDepositAmount { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal? InterestAmount { get; set; }
        public string? MemberNumber { get; set; }
        public decimal? FirstSavingAmount { get; set; }
        public DateTime? FirstSavingDate { get; set; }
        public decimal? LastSavingAmount { get; set; }
        public DateTime? LastSavingDate { get; set; }
        public int? MemberAccountID { get; set; }
        public int? MemberId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? AccountStatus { get; set; }

        public Member? Member { get; set; }
        public MemberAccount? MemberAccount { get; set; }
    }

    public class MemberSavingDefinition
    {
        [Key]
        public int MemberSavingDefinitionId { get; set; }
        public string? MemberNumber { get; set; }
        public int? AccountTypeId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? MemberAccountID { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? CuurentStatus { get; set; }

        public AccountType? AccountType { get; set; }
        public Member? Member { get; set; }
        public IEnumerable<MemberSaving_Deposit>? MemberSaving_Deposit { get; set; }
    }

    public class MemberGuarantor
    {
        [Key]
        public int GuarantorID { get; set; }
        public string? MemberNumber { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? MonthlyRevenue { get; set; }
        public decimal? MonthlyProfit { get; set; }
        public string? Relationship { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public Member? Member { get; set; }
    }

    public class MemberBank
    {
        [Key]
        public int CBId { get; set; }
        public string? MemberNumber { get; set; }
        public string? CBAccountNumber { get; set; }
        public string? CBAccountName { get; set; }
        public string? CBName { get; set; }
        public string? CBSortCode { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? MemberId { get; set; }

        public Member? Member { get; set; }
    }

    public class MemberAccount
    {
        [Key]
        public int MemberAccountID { get; set; }
        public string? AccountNo { get; set; }
        public string? UserAccountNo { get; set; }
        public int? MemberId { get; set; }
        public string? MemberNumber { get; set; }
        public string? AccountName { get; set; }
        public int? AccountTypeID { get; set; }
        public DateTime? DateOpened { get; set; }
        public decimal? InitialDeposit { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByStaffID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? AccountStatus { get; set; }
        public string? LockStatus { get; set; }
        public string? BVNNumber { get; set; }

        public Member? Member { get; set; }
        public IEnumerable<MemberSavingSummary>? MemberSavingSummaries { get; set; }
        public IEnumerable<MemberTransactionBalance>? MemberTransactionBalances { get; set; }
        public IEnumerable<MemberWithdrawal>? MemberWithdrawals { get; set; }
        public IEnumerable<MemberSaving_Deposit>? MemberSaving_Deposit { get; set; }
    }

    public class Member
    {
        public string? MemberNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Nationality { get; set; }
        public int? RoleId { get; set; }
        public int? StaffID { get; set; }
        public string? RoleDesc { get; set; }
        public string? Password { get; set; }
        public string? MemberStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        [Key]
        public int MemberId { get; set; }
        public int? MemberDeptID { get; set; }
        public string? MaritalStatus { get; set; }
        public int? StateOfOrigin { get; set; }
        public string? HomeTel { get; set; }
        public string? SignatureImage { get; set; }
        public string? Position { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyResp { get; set; }
        public DateTime? HiredDate { get; set; }
        public string? photo { get; set; }
        public int? wfstatus { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? IntroducedBy { get; set; }
        public string? NextofKin { get; set; }
        public string? NextKinAddress { get; set; }
        public string? NextKinPhoneNumber { get; set; }
        public string? AcceptPolicy { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ManagementApproval { get; set; }
        public int? LocationID { get; set; }
        public decimal? InitialContributionAmount { get; set; }
        public decimal? CurrentContributionAmount { get; set; }
        public DateTime? ContributionStartDate { get; set; }
        public decimal? Salary { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? NINNumber { get; set; }
        public string? ReferalCode { get; set; }
        public int? MemberRankId { get; set; }
        public int? CurrentAccumulatedPoints { get; set; }
        public string? UserId { get; set; }
        public string? SendSMS { get; set; }
        public string? CreatedByUserRole { get; set; }
        [NotMapped]
        public IFormFile? MemberPhoto { get; set; }
        [NotMapped]
        public IFormFile? SignaturePhoto { get; set; }

        public IEnumerable<CoorperatorBooklet>? CoorperatorBooklets { get; set; }
        public IEnumerable<LoanRepaymentPlan>? LoanRepaymentPlans { get; set; }
        public IEnumerable<LoanRepaymentSummary>? LoanRepaymentSummaries { get; set; }
        public Role? Role { get; set; }
        public Staff? Staff { get; set; }
        public IEnumerable<MemberAccount>? MemberAccounts { get; set; }
        public IEnumerable<MemberBank>? MemberBanks { get; set; }
        public IEnumerable<MemberGuarantor>? MemberGuarantors { get; set; }
        public IEnumerable<MemberSavingDefinition>? MemberSavingDefinitions { get; set; }
        public IEnumerable<MemberSavingSummary>? MemberSavingSummaries { get; set; }
        public IEnumerable<MemberTransactionBalance>? MemberTransactionBalances { get; set; }
        public IEnumerable<MemberWithdrawal>? MemberWithdrawals { get; set; }
        public IEnumerable<tblReceipt>? tblReceipts { get; set; }
        public IEnumerable<LoanRepaymentMonthlySchedule>? LoanRepaymentMonthlySchedules { get; set; }
        public IEnumerable<LoanRepayment>? LoanRepayments { get; set; }
        public IEnumerable<MemberSaving_Deposit>? MemberSaving_Deposit { get; set; }
    }

    public class Location
    {
        [Key]
        public int LocationID { get; set; }
        public string? LocationName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class LoanType
    {
        [Key]
        public int LoanTypeId { get; set; }
        public string? LoanDesc { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? FormFee { get; set; }
        public string? Settings { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<LoanApplication>? LoanApplications { get; set; }
        public IEnumerable<LoanRepaymentSummary>? LoanRepaymentSummaries { get; set; }
    }

    public class LoanRepaymentSummary
    {
        [Key]
        public int LoanRepaymentSummaryId { get; set; }
        public int? LoanTypeId { get; set; }
        public string? MemberNumber { get; set; }
        public int? FirstRepaymentMonth { get; set; }
        public int? FirstRepaymentYear { get; set; }
        public int? LastRepaymentMonth { get; set; }
        public int? LastRepaymentYear { get; set; }
        public decimal? TotalLoanAmount { get; set; }
        public decimal? TotalAmountRepaid { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public int? NumberofMonthsPaid { get; set; }
        public int? LoanDurationInMonths { get; set; }
        public int? LoanApplicationId { get; set; }
        public int? LoanRepaymentPlanId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? PaymentStatus { get; set; }
        public int? RepaymentPlanTypeId { get; set; }
        public decimal? ExpectedTotalAmount { get; set; }
        public int? AccountTypeId { get; set; }

        public LoanApplication? LoanApplication { get; set; }
        public LoanType? LoanType { get; set; }
        public Member? Member { get; set; }
    }
    public class LoanRepaymentPlanType
    {
        [Key]
        public int RepaymentPlanTypeId { get; set; }
        public string? RepaymentPlanTypeDesc { get; set; }
        public string? MonthCovered { get; set; }
        public decimal? InterestRate { get; set; }
        public string? RateDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<LoanRepaymentPlan>? LoanRepaymentPlans { get; set; }
        public IEnumerable<LoanRepaymentMonthlySchedule>? LoanRepaymentMonthlySchedules { get; set; }
    }
    public class LoanRepaymentPlan
    {
        [Key]
        public int LoanRepaymentPlanId { get; set; }
        public int? RepaymentPlanTypeId { get; set; }
        public int? LoanApplicationId { get; set; }
        public string? MemberNumber { get; set; }
        public string? FirstMonthlyRepayment { get; set; }
        public string? LastMonthlyRepayment { get; set; }
        public decimal? TotalRepaymentAmount { get; set; }
        public decimal? MonthlyRepaymentAmount { get; set; }
        public DateTime? FirstRDate { get; set; }
        public DateTime? SecondRDate { get; set; }
        public DateTime? ThirdRDate { get; set; }
        public DateTime? FourthRDate { get; set; }
        public DateTime? FifthRDate { get; set; }
        public DateTime? SixthRDate { get; set; }
        public DateTime? SeventhRDate { get; set; }
        public DateTime? EightRDate { get; set; }
        public DateTime? NinethRDate { get; set; }
        public DateTime? TenthRDate { get; set; }
        public DateTime? EleventhRDate { get; set; }
        public DateTime? TwelvethRDate { get; set; }
        public DateTime? ThirteenthRDate { get; set; }
        public DateTime? FourteenthRDate { get; set; }
        public DateTime? FifteenthRDate { get; set; }
        public DateTime? SixteenthRDate { get; set; }
        public DateTime? SeventeenthRDate { get; set; }
        public DateTime? EighteenthRDate { get; set; }
        public DateTime? NineteenthRDate { get; set; }
        public DateTime? TwentiethRDate { get; set; }
        public DateTime? TwentyFirstRDate { get; set; }
        public DateTime? TwentySecondRDate { get; set; }
        public DateTime? TwentyThirdRDate { get; set; }
        public DateTime? TwentyFourthRDate { get; set; }
        public int? NumberofMonth { get; set; }
        public string? RepaymentDay { get; set; }
        public string? RepaymentYear { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string? RepaymentDuration { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public LoanApplication? LoanApplication { get; set; }
        public Member? Member { get; set; }
        public LoanRepaymentPlanType? LoanRepaymentPlanType { get; set; }
    }
    public class LoanApplication
    {
        [Key]
        public int LoanApplicationId { get; set; }
        public string? LoanDesc { get; set; }
        public int? LoanTypeId { get; set; }
        public int? AccountTypeId { get; set; }
        public string? MemberNumber { get; set; }
        public decimal? AmountApplied { get; set; }
        public decimal? AmountApproved { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public decimal? MonthlyProfit { get; set; }
        public decimal? MonthlyRevenue { get; set; }
        public decimal? QTESavingsAccountBalance { get; set; }
        public string? Remarks { get; set; }
        public DateTime? DateApplied { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? LoanStatus { get; set; }
        public int? FirstGuarrantorId { get; set; }
        public decimal? FGuarrantorContribution { get; set; }
        public string? FGuarrantorAcceptance { get; set; }
        public string? FGuarrantorAcceptanceDate { get; set; }
        public int? SecondGuarrantorId { get; set; }
        public decimal? SecondGuarrantorContribution { get; set; }
        public string? SecondGuarrantorAcceptance { get; set; }
        public string? SecondGuarrantorAcceptanceDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? LoanPeriod { get; set; }
        public int? ExtensionLoanPeriod { get; set; }
        public string? ExtensionReason { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public decimal? MonthlyRepayment { get; set; }
        public string? PresidentResponse { get; set; }
        public DateTime? PresidentResponseDate { get; set; }
        public string? PresidentNumber { get; set; }
        public string? SecretaryRespone { get; set; }
        public DateTime? SecretaryResponeDate { get; set; }
        public string? SecretaryNumber { get; set; }
        public string? TreasuryResponse { get; set; }
        public DateTime? TreasuryResponseDate { get; set; }
        public string? TreasuryNumber { get; set; }
        public string? FCreditMemberResponse { get; set; }
        public DateTime? FCreditMemberResponseDate { get; set; }
        public string? FCreditMemberNumber { get; set; }
        public string? SecondCreditMemberResponse { get; set; }
        public DateTime? SecondCreditMemberResponseDate { get; set; }
        public string? SecondCreditMemberNumber { get; set; }
        public string? ConfirmAmountReceived { get; set; }
        public DateTime? ConfirmDateReceived { get; set; }
        public string? ConfirmReceivedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public decimal? OutstandingBalance { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal? InterestRate { get; set; }
        public string? RepaymentStatus { get; set; }
        public string? BankStatement { get; set; }
        public string? CollateralDoc { get; set; }
        public string? BusinessPhoneNumber { get; set; }
        public string? RepaymentTerms { get; set; }
        public string? PurposeOfLoan { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public decimal? ApplicationFee { get; set; }
        public string? ApplicationFeeStatus { get; set; }
        public string? CreatedByUserRole { get; set; }
        public DateTime? HODDateReviewed { get; set; }
        public int? HODReviewedBy { get; set; }
        public string? HODReviewComment { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ApprovalComment { get; set; }
        public int? ApprovalStatus { get; set; }

        public LoanType? LoanType { get; set; }
        public Staff? Staff { get; set; }
        public IEnumerable<LoanRepaymentPlan>? LoanRepaymentPlans { get; set; }
        public IEnumerable<LoanRepaymentSummary>? LoanRepaymentSummaries { get; set; }
        public IEnumerable<LoanRepaymentMonthlySchedule>? LoanRepaymentMonthlySchedules { get; set; }
        public IEnumerable<LoanRepayment>? LoanRepayments { get; set; }
    }
    public class LeadPosition
    {
        [Key]
        public int LeadPositionId { get; set; }
        public string? LeadPositionDesc { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class EmailMessage
    {
        [Key]
        public int EmailMessageID { get; set; }
        public string? Emaildescription { get; set; }
        public DateTime? Emailsentdate { get; set; }
        public int? Emailsender { get; set; }
        public string? Emailtile { get; set; }
        public string? Emailattachment { get; set; }
        public int? EmailReceiver { get; set; }
        public string? ReceiverType { get; set; }
        public string? Status { get; set; }
        public string? MessageType { get; set; }
        public string? WFStatus { get; set; }
        public string? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? EmailSentDateTime { get; set; }
        public int? OrganizationId { get; set; }
        public string? Emailattachment1 { get; set; }
        public string? Emailattachment2 { get; set; }
        public string? Emailattachment3 { get; set; }
        public string? Emailattachment4 { get; set; }
        public string? FileName { get; set; }
        public string? FileName1 { get; set; }
        public string? FileName2 { get; set; }
        public string? FileName3 { get; set; }
        public string? FileName4 { get; set; }
    }
    public class Department
    {
        [Key]
        public int Departmentid { get; set; }
        public string? DepartmentName { get; set; }
        public int? DepartmentHead { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? DepartmentParentId { get; set; }
    }
    public class Delegation
    {
        [Key]
        public int DelegationID { get; set; }
        public int? DelegatedBy { get; set; }
        public string? Description { get; set; }
        public string? CooperatorNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? RoleId { get; set; }
    }
    public class DeductionType
    {
        [Key]
        public int DeductionTypeId { get; set; }
        public string? DeductionDesc { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<CoorperatorBooklet>? CoorperatorBooklets { get; set; }
    }
    public class CoorperatorBooklet
    {
        [Key]
        public int CoorperatorBookletId { get; set; }
        public decimal? ContributionAmount { get; set; }
        public decimal? DeductionAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModfiedDate { get; set; }
        public string? MemberNumber { get; set; }
        public int? ContributionTypeId { get; set; }
        public int? DeductionTypeId { get; set; }
        public string? Month { get; set; }
        public int? Year { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public ContributionType? ContributionType { get; set; }
        public DeductionType? DeductionType { get; set; }
        public Member? Member { get; set; }
    }
    public class ContributionType
    {
        [Key]
        public int ContributionTypeId { get; set; }
        public string? ContributionDesc { get; set; }
        public decimal? ContributionTypeAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }

        public IEnumerable<CoorperatorBooklet>? CoorperatorBooklets { get; set; }
    }
    public class ContactU
    {
        [Key]
        public int ContactUsId { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? FullName { get; set; }
        public string? Contentbody { get; set; }
        public string? Createdby { get; set; }
        public string? SentDate { get; set; }
    }
    public class CompanyAccountDetail
    {
        [Key]
        public int AccountID { get; set; }
        public string? AccountName { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public string? SortCode { get; set; }
        public string? BranchAddress { get; set; }
        public string? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
    }
    public class BirthDayCerebration
    {
        public string? MemberNumber { get; set; }
        public DateTime? DOB { get; set; }
        public string? BDay { get; set; }
        public string? Name { get; set; }
        [Key]
        public int BirthDayCerebrationID { get; set; }
        public string? CreatedDate { get; set; }
        public int? CreatedStaffID { get; set; }
        public string? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public int? OrganizationId { get; set; }
        public int? LocationId { get; set; }
        public Staff? Staff { get; set; }
    }
    public class BillingType
    {
        [Key]
        public int BillingTypeId { get; set; }
        public string? BillingTypeDesc { get; set; }
    }
    public class audittrailHistory
    {
        [Key]
        public int audittrailHistoryId { get; set; }
        public int? audittrailid { get; set; }
        public string? tablename { get; set; }
        public string? operation { get; set; }
        public DateTime? occurreddate { get; set; }
        public string? fieldnameNData1 { get; set; }
        public string? fieldnameNData2 { get; set; }
        public string? fieldnameNData3 { get; set; }
        public string? fieldnameNData4 { get; set; }
        public string? fieldnameNData5 { get; set; }
        public string? fieldnameNData6 { get; set; }
        public string? fieldnameNData7 { get; set; }
        public string? fieldnameNData8 { get; set; }
        public string? fieldnameNData9 { get; set; }
        public string? fieldnameNData10 { get; set; }
        public string? fieldnameNData11 { get; set; }
        public string? fieldnameNData12 { get; set; }
        public string? fieldnameNData13 { get; set; }
        public string? fieldnameNData14 { get; set; }
        public string? fieldnameNData15 { get; set; }
        public string? fieldnameNData16 { get; set; }
        public string? fieldnameNData17 { get; set; }
        public string? fieldnameNData18 { get; set; }
        public string? fieldnameNData19 { get; set; }
        public string? fieldnameNData20 { get; set; }
        public string? fieldnameNData21 { get; set; }
        public string? fieldnameNData22 { get; set; }
        public string? fieldnameNData23 { get; set; }
        public string? fieldnameNData24 { get; set; }
        public string? fieldnameNData25 { get; set; }

        public audittrail? audittrail { get; set; }
    }
    public class audittrail
    {
        [Key]
        public int audittrailid { get; set; }
        public string? tablename { get; set; }
        public string? operation { get; set; }
        public DateTime? occurreddate { get; set; }
        public string? timeoccurred { get; set; }
        public string? performedbyname { get; set; }
        public int? performedbyid { get; set; }
        public string? fieldname { get; set; }
        public string? oldvalue { get; set; }
        public string? newvalue { get; set; }

        public IEnumerable<audittrailHistory>? audittrailHistories { get; set; }
    }
    public class AccountType
    {
        [Key]
        public int AccountTypeID { get; set; }
        public string? AccountTypeDesc { get; set; }
        public decimal? MinimumBalance { get; set; }
        public int? HasFixedRate { get; set; }
        public int? NonAccessibleTill { get; set; }
        public decimal? InterestRate { get; set; }
        public int? StaffID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? OrganizationId { get; set; }
        public string? ReferenceUniqueId { get; set; }
        public string? AccountNumberSeq { get; set; }

        public Staff? Staff { get; set; }
        public IEnumerable<MemberSavingDefinition>? MemberSavingDefinitions { get; set; }
        public IEnumerable<MemberSaving_Deposit>? MemberSaving_Deposit { get; set; }
    }
}
