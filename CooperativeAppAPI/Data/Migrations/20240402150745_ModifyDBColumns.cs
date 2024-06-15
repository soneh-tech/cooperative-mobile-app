using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CooperativeAppAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDBColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string proc_1 = "CREATE PROCEDURE [dbo].[AuditTrailProc]\r\n-- Add the parameters for the stored procedure here\r\n(@tabId int, @tabName nvarchar(50), @opr nvarchar(50), @date nvarchar(50))\r\nAS\r\nBEGIN\r\n-- SET NOCOUNT ON added to prevent extra result sets from\r\n-- interfering with SELECT statements.\r\nSET NOCOUNT ON;\r\n\r\n-- Insert statements for procedure here\r\nDECLARE @OldValues nvarchar(MAX), @ModifiedValues nvarchar(MAX), @ModifiedFields nvarchar(MAX)\r\n\r\nSELECT @tabId = tm.tableId, @tabName = tm.tablename, @OldValues = tm.OriginalValue, @ModifiedValues = tm.ModifiedValue, @ModifiedFields = tm.ModifiedFields\r\nFROM tblAuditTrailMod tm\r\nWHERE tm.tableId = @tabId AND tm.tablename = @tabName AND tm.operation = @opr AND tm.occurreddate = @date\r\n\r\n--Check If Record already Exists in tblaudittrailMod table\r\nIF EXISTS (\r\nSELECT DISTINCT *\r\nfrom dbo.tblaudittrailMod\r\nWHERE tableId = @tabId AND tablename = @tabName AND operation = @opr AND occurreddate = @date\r\n)\r\nBEGIN\r\nUPDATE tblaudittrailMod SET OriginalValue = @OldValues, ModifiedValue = @ModifiedValues, ModifiedFields = @ModifiedFields\r\nWHERE tableId = @tabId AND tablename = @tabName AND operation = @opr AND occurreddate = @date\r\n\r\nEND\r\n--DELETE FROM dbo.tblAuditTrailMod WHERE tableId = @tabId AND tablename = @tabName AND operation = @opr AND occurreddate = @date\r\nEND\r\n\r\nGO\r\n/****** Object:  StoredProcedure [dbo].[ProcAutoFlagOverDuePayment]    Script Date: 03/13/2024 3:18:22 PM ******/\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\n\r\n";

            string proc_2 = "CREATE PROCEDURE [dbo].[ProcAutoFlagOverDuePayment]\r\n-- Add the parameters for the stored procedure here\r\n(@RepaymentId int,@MemberNumber nvarchar(50), @PaymentDueDate date, @PaymentStatus nvarchar(50), @date date, @LoanId int)\r\nAS\r\nBEGIN\r\n-- SET NOCOUNT ON added to prevent extra result sets from\r\n-- interfering with SELECT statements.\r\nSET NOCOUNT ON;\r\n\r\n-- Insert statements for procedure here\r\nDeclare @DueDate Date = CONVERT(date,GETDATE())\r\nDeclare @Currentdatetime Date = CONVERT(date, GETDATE())\r\nDeclare @Messages nvarchar(50) = 'Overdue Payment'\r\nDeclare @AmountDue decimal(18,2)\r\n\r\n\r\nSELECT @Currentdatetime\r\nSELECT @DueDate\r\nIF(@Currentdatetime >= @DueDate)\r\nBEGIN\r\n\r\nSELECT @AmountDue = (Select SUM(sal.MonthRepaymentAmount) From [dbo].[LoanRepaymentMonthlySchedules] sal where sal.RepaymentPlanId = @RepaymentId AND (sal.RepaymentStatus = 'Scheduled' OR sal.RepaymentStatus = 'Pending' OR RepaymentStatus = 'Unpaid') AND sal.MemberNumber = @MemberNumber GROUP BY sal.MemberNumber, sal.LoanApplicationId)\r\n\r\nIF(@AmountDue > 0)\r\nBEGIN\r\nSELECT @Messages\r\nUPDATE dbo.LoanRepaymentMonthlySchedules SET\r\nRepaymentStatus = @Messages\r\nWHERE MemberNumber = @MemberNumber AND RepaymentPlanId = @RepaymentId AND PaymentDueDate <= @DueDate AND (RepaymentStatus = 'Scheduled' OR RepaymentStatus = 'Unpaid' OR RepaymentStatus = 'Pending')\r\n\r\nEND\r\n\r\nEND\r\n\r\n\r\nEND\r\n\r\n\r\nGO\r\n/****** Object:  Table [dbo].[AccountType]    Script Date: 03/13/2024 3:18:22 PM ******/\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO";

            migrationBuilder.Sql(proc_1);
            migrationBuilder.Sql(proc_2);

            migrationBuilder.CreateTable(
                name: "audittrail",
                columns: table => new
                {
                    audittrailid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tablename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    occurreddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    timeoccurred = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    performedbyname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    performedbyid = table.Column<int>(type: "int", nullable: true),
                    fieldname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    oldvalue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    newvalue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audittrail", x => x.audittrailid);
                });

            migrationBuilder.CreateTable(
                name: "BillingType",
                columns: table => new
                {
                    BillingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillingTypeDesc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingType", x => x.BillingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAccountDetail",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAccountDetail", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    ContactUsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contentbody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.ContactUsId);
                });

            migrationBuilder.CreateTable(
                name: "ContributionType",
                columns: table => new
                {
                    ContributionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContributionDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContributionTypeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionType", x => x.ContributionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "DeductionType",
                columns: table => new
                {
                    DeductionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeductionDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductionType", x => x.DeductionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Delegation",
                columns: table => new
                {
                    DelegationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DelegatedBy = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CooperatorNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delegation", x => x.DelegationID);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Departmentid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentHead = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DepartmentParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Departmentid);
                });

            migrationBuilder.CreateTable(
                name: "EmailMessage",
                columns: table => new
                {
                    EmailMessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emaildescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailsentdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Emailsender = table.Column<int>(type: "int", nullable: true),
                    Emailtile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailattachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailReceiver = table.Column<int>(type: "int", nullable: true),
                    ReceiverType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WFStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    EmailSentDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    Emailattachment1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailattachment2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailattachment3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emailattachment4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName4 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailMessage", x => x.EmailMessageID);
                });

            migrationBuilder.CreateTable(
                name: "LeadPosition",
                columns: table => new
                {
                    LeadPositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadPositionDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadPosition", x => x.LeadPositionId);
                });

            migrationBuilder.CreateTable(
                name: "LoanPenalty",
                columns: table => new
                {
                    LoanPenaltyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPenalty", x => x.LoanPenaltyId);
                });

            migrationBuilder.CreateTable(
                name: "LoanRepaymentPlanType",
                columns: table => new
                {
                    RepaymentPlanTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepaymentPlanTypeDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthCovered = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RateDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepaymentPlanType", x => x.RepaymentPlanTypeId);
                });

            migrationBuilder.CreateTable(
                name: "LoanType",
                columns: table => new
                {
                    LoanTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FormFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanType", x => x.LoanTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "NetworkInfo",
                columns: table => new
                {
                    NC_PK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NC_SMTPName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NC_UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NC_NetworkPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NC_PortNumber = table.Column<int>(type: "int", nullable: true),
                    NetworkSSL = table.Column<bool>(type: "bit", nullable: true),
                    NC_CUST_FK = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkInfo", x => x.NC_PK);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    OrgId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncoperationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Motto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationInitials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZoomEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrgId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationFee",
                columns: table => new
                {
                    RegistrationFeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationFee1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationFee", x => x.RegistrationFeeId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Roleid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rolename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Roleid);
                });

            migrationBuilder.CreateTable(
                name: "StaffCategory",
                columns: table => new
                {
                    StaffCatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffCategory", x => x.StaffCatID);
                });

            migrationBuilder.CreateTable(
                name: "StaffPension",
                columns: table => new
                {
                    PensionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PensionEmployeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PensionEmployerRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPension", x => x.PensionId);
                });

            migrationBuilder.CreateTable(
                name: "StaffTax",
                columns: table => new
                {
                    TaxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffTax", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketts",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketPriority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailSentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketts", x => x.TicketId);
                });

            migrationBuilder.CreateTable(
                name: "tblAdministrativeCharge",
                columns: table => new
                {
                    AdministrativeChargeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChargeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChargeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChargeTableId = table.Column<int>(type: "int", nullable: true),
                    ChargeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChargeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAdministrativeCharge", x => x.AdministrativeChargeId);
                });

            migrationBuilder.CreateTable(
                name: "tblaudittrail",
                columns: table => new
                {
                    audittrailid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tablename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    occurreddate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeoccurred = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    performedbyname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    performedbyid = table.Column<int>(type: "int", nullable: true),
                    fieldname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    oldvalue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    newvalue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tableId = table.Column<int>(type: "int", nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedFields = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblaudittrail", x => x.audittrailid);
                });

            migrationBuilder.CreateTable(
                name: "tblAuditTrailMod",
                columns: table => new
                {
                    AuditTrailModId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tableId = table.Column<int>(type: "int", nullable: true),
                    OriginalValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tablename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    occurreddate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAuditTrailMod", x => x.AuditTrailModId);
                });

            migrationBuilder.CreateTable(
                name: "tblBank",
                columns: table => new
                {
                    BankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ussd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBank", x => x.BankId);
                });

            migrationBuilder.CreateTable(
                name: "tblExpensesCategory",
                columns: table => new
                {
                    ExpensesCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpensesCategoryDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblExpensesCategory", x => x.ExpensesCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "tblExpensesType",
                columns: table => new
                {
                    ExpensesTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpensesTypeDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpensesCategoryID = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblExpensesType", x => x.ExpensesTypeID);
                });

            migrationBuilder.CreateTable(
                name: "tblGeneralJournal",
                columns: table => new
                {
                    GeneralJournalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JournalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DebitDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DebitBalBD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditBalBD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ChartAccount = table.Column<int>(type: "int", nullable: true),
                    TransactionYear = table.Column<int>(type: "int", nullable: true),
                    TransactionMonth = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblGeneralJournal", x => x.GeneralJournalID);
                });

            migrationBuilder.CreateTable(
                name: "tblInterestOnAccount",
                columns: table => new
                {
                    InterestOnAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    CreditStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblInterestOnAccount", x => x.InterestOnAccountId);
                });

            migrationBuilder.CreateTable(
                name: "tblLedger",
                columns: table => new
                {
                    LedgerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID = table.Column<int>(type: "int", nullable: true),
                    LedgerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LDebitDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LDebitBalBD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LCreditDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LCreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LCreditBalBD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLedger", x => x.LedgerID);
                });

            migrationBuilder.CreateTable(
                name: "tblNotificationMethod",
                columns: table => new
                {
                    NotificationMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationMethodDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblNotificationMethod", x => x.NotificationMethodID);
                });

            migrationBuilder.CreateTable(
                name: "tblOTPHistory",
                columns: table => new
                {
                    OTPHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ctime_Stamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OTPCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TansactionRecordId = table.Column<int>(type: "int", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOTPHistory", x => x.OTPHistoryId);
                });

            migrationBuilder.CreateTable(
                name: "tblShareHoldersEquity",
                columns: table => new
                {
                    ShareHoldersEquityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvestmentCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentageShare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetainedEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvestmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblShareHoldersEquity", x => x.ShareHoldersEquityId);
                });

            migrationBuilder.CreateTable(
                name: "tblSMSMessage",
                columns: table => new
                {
                    SMSMessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SMSMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMSreceiver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMSMessageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSuccess = table.Column<int>(type: "int", nullable: true),
                    TotalFailure = table.Column<int>(type: "int", nullable: true),
                    TotalCharged = table.Column<int>(type: "int", nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSMSMessage", x => x.SMSMessageID);
                });

            migrationBuilder.CreateTable(
                name: "tblSMSNetworkInfo",
                columns: table => new
                {
                    SMSNetworkInoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HttpAPIKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    APIUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSMSNetworkInfo", x => x.SMSNetworkInoID);
                });

            migrationBuilder.CreateTable(
                name: "tblState",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblState", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "tempLoanCalculationtable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalPrincipalBal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RemainingPrincipalBal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OverallTotalPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentageInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ScheduleNumber = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    ReOrderLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tempLoanCalculationtable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "temptblBalanceSheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountingCode = table.Column<int>(type: "int", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionMonth = table.Column<int>(type: "int", nullable: true),
                    TransactionYear = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temptblBalanceSheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "temptblIncomeandExpenditures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHARTOFACCOUNT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionMonth = table.Column<int>(type: "int", nullable: true),
                    TransactionYear = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temptblIncomeandExpenditures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDelegation",
                columns: table => new
                {
                    DelegationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DelegatedBy = table.Column<int>(type: "int", nullable: true),
                    DelegationDept = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDelegation", x => x.DelegationID);
                });

            migrationBuilder.CreateTable(
                name: "UserReferralTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferralCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MyReferralStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReferralTracking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WFStatus",
                columns: table => new
                {
                    WFStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WFStatus", x => x.WFStatusID);
                });

            migrationBuilder.CreateTable(
                name: "audittrailHistory",
                columns: table => new
                {
                    audittrailHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    audittrailid = table.Column<int>(type: "int", nullable: true),
                    tablename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    operation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    occurreddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fieldnameNData1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData7 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData8 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData9 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData11 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData12 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData13 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData14 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData15 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData16 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData17 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData18 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData19 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData20 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData21 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData22 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData23 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData24 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldnameNData25 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audittrailHistory", x => x.audittrailHistoryId);
                    table.ForeignKey(
                        name: "FK_audittrailHistory_audittrail_audittrailid",
                        column: x => x.audittrailid,
                        principalTable: "audittrail",
                        principalColumn: "audittrailid");
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffCatID = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    StaffStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfOrigin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeTel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    RoleDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staff_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Roleid");
                });

            migrationBuilder.CreateTable(
                name: "SupportTicketReponses",
                columns: table => new
                {
                    TicketReponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketPriority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailSentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketId = table.Column<int>(type: "int", nullable: true),
                    SupportTickettTicketId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTicketReponses", x => x.TicketReponseId);
                    table.ForeignKey(
                        name: "FK_SupportTicketReponses_SupportTicketts_SupportTickettTicketId",
                        column: x => x.SupportTickettTicketId,
                        principalTable: "SupportTicketts",
                        principalColumn: "TicketId");
                });

            migrationBuilder.CreateTable(
                name: "tblLGA",
                columns: table => new
                {
                    LGAId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LGAName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LGACode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    tblStateStateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblLGA", x => x.LGAId);
                    table.ForeignKey(
                        name: "FK_tblLGA_tblState_tblStateStateId",
                        column: x => x.tblStateStateId,
                        principalTable: "tblState",
                        principalColumn: "StateId");
                });

            migrationBuilder.CreateTable(
                name: "AccountType",
                columns: table => new
                {
                    AccountTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTypeDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasFixedRate = table.Column<int>(type: "int", nullable: true),
                    NonAccessibleTill = table.Column<int>(type: "int", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumberSeq = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountType", x => x.AccountTypeID);
                    table.ForeignKey(
                        name: "FK_AccountType_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "BirthDayCerebration",
                columns: table => new
                {
                    BirthDayCerebrationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedStaffID = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthDayCerebration", x => x.BirthDayCerebrationID);
                    table.ForeignKey(
                        name: "FK_BirthDayCerebration_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "LoanApplication",
                columns: table => new
                {
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanTypeId = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountApproved = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlyProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    QTESavingsAccountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateApplied = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoanStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstGuarrantorId = table.Column<int>(type: "int", nullable: true),
                    FGuarrantorContribution = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FGuarrantorAcceptance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FGuarrantorAcceptanceDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondGuarrantorId = table.Column<int>(type: "int", nullable: true),
                    SecondGuarrantorContribution = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SecondGuarrantorAcceptance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondGuarrantorAcceptanceDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoanPeriod = table.Column<int>(type: "int", nullable: true),
                    ExtensionLoanPeriod = table.Column<int>(type: "int", nullable: true),
                    ExtensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MonthlyRepayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PresidentResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresidentResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PresidentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecretaryRespone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecretaryResponeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecretaryNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreasuryResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreasuryResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TreasuryNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FCreditMemberResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FCreditMemberResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FCreditMemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondCreditMemberResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondCreditMemberResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecondCreditMemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmAmountReceived = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmDateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CollateralDoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurposeOfLoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ApplicationFeeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HODDateReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HODReviewedBy = table.Column<int>(type: "int", nullable: true),
                    HODReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplication", x => x.LoanApplicationId);
                    table.ForeignKey(
                        name: "FK_LoanApplication_LoanType_LoanTypeId",
                        column: x => x.LoanTypeId,
                        principalTable: "LoanType",
                        principalColumn: "LoanTypeId");
                    table.ForeignKey(
                        name: "FK_LoanApplication_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    RoleDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberDeptID = table.Column<int>(type: "int", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfOrigin = table.Column<int>(type: "int", nullable: true),
                    HomeTel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyResp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wfstatus = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IntroducedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextofKin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextKinAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextKinPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ManagementApproval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    InitialContributionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentContributionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContributionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NINNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberRankId = table.Column<int>(type: "int", nullable: true),
                    CurrentAccumulatedPoints = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendSMS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Member_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Roleid");
                    table.ForeignKey(
                        name: "FK_Member_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "ResetPass",
                columns: table => new
                {
                    PasswordChangeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    PassChangeStaffID = table.Column<int>(type: "int", nullable: true),
                    PassChangeReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HRcomment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPass", x => x.PasswordChangeID);
                    table.ForeignKey(
                        name: "FK_ResetPass_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "Staff_Sal",
                columns: table => new
                {
                    StaffSalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    BeginGrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BeginNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HomeAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UtilityAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentGrosstSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PensionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headDeptId = table.Column<int>(type: "int", nullable: true),
                    headDeptremarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headDeptdatemodified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    CurrentSalaryBeginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDateIncreaseSalary = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff_Sal", x => x.StaffSalaryId);
                    table.ForeignKey(
                        name: "FK_Staff_Sal_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "StaffSalaryDefinition",
                columns: table => new
                {
                    StaffSalaryDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    BeginGrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BeginNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HomeAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UtilityAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentGrosstSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentSalaryBeginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headDeptId = table.Column<int>(type: "int", nullable: true),
                    headDeptremarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headDeptdatemodified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDateIncreaseSalary = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffSalaryDefinition", x => x.StaffSalaryDefinitionId);
                    table.ForeignKey(
                        name: "FK_StaffSalaryDefinition_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "tblAsset",
                columns: table => new
                {
                    AssetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    PurchasedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmountPurchased = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DepreciationPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchasedBy = table.Column<int>(type: "int", nullable: true),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptSampleCopy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAsset", x => x.AssetID);
                    table.ForeignKey(
                        name: "FK_tblAsset_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "tblExpenses",
                columns: table => new
                {
                    ExpensesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpensesCategoryID = table.Column<int>(type: "int", nullable: true),
                    ExpensesTypeID = table.Column<int>(type: "int", nullable: true),
                    ExpensesDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ExpensesByWho = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    TransactionYear = table.Column<int>(type: "int", nullable: true),
                    TransactionMonth = table.Column<int>(type: "int", nullable: true),
                    ModeOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccruedPrepaid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tblExpensesCategoryExpensesCategoryID = table.Column<int>(type: "int", nullable: true),
                    tblExpensesTypeExpensesTypeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblExpenses", x => x.ExpensesId);
                    table.ForeignKey(
                        name: "FK_tblExpenses_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_tblExpenses_tblExpensesCategory_tblExpensesCategoryExpensesCategoryID",
                        column: x => x.tblExpensesCategoryExpensesCategoryID,
                        principalTable: "tblExpensesCategory",
                        principalColumn: "ExpensesCategoryID");
                    table.ForeignKey(
                        name: "FK_tblExpenses_tblExpensesType_tblExpensesTypeExpensesTypeID",
                        column: x => x.tblExpensesTypeExpensesTypeID,
                        principalTable: "tblExpensesType",
                        principalColumn: "ExpensesTypeID");
                });

            migrationBuilder.CreateTable(
                name: "tblSessionLog",
                columns: table => new
                {
                    SessionLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionID = table.Column<int>(type: "int", nullable: true),
                    SessionStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSessionLog", x => x.SessionLogID);
                    table.ForeignKey(
                        name: "FK_tblSessionLog_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "CoorperatorBooklet",
                columns: table => new
                {
                    CoorperatorBookletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContributionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModfiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContributionTypeId = table.Column<int>(type: "int", nullable: true),
                    DeductionTypeId = table.Column<int>(type: "int", nullable: true),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoorperatorBooklet", x => x.CoorperatorBookletId);
                    table.ForeignKey(
                        name: "FK_CoorperatorBooklet_ContributionType_ContributionTypeId",
                        column: x => x.ContributionTypeId,
                        principalTable: "ContributionType",
                        principalColumn: "ContributionTypeId");
                    table.ForeignKey(
                        name: "FK_CoorperatorBooklet_DeductionType_DeductionTypeId",
                        column: x => x.DeductionTypeId,
                        principalTable: "DeductionType",
                        principalColumn: "DeductionTypeId");
                    table.ForeignKey(
                        name: "FK_CoorperatorBooklet_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "LoanRepaymentMonthlySchedules",
                columns: table => new
                {
                    RepaymentPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    RepaymentPlanTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemainingPrincipalBal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyPrincipalBal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyInterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepaymentScheduleNumber = table.Column<int>(type: "int", nullable: true),
                    RepaymentYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberofMonths = table.Column<int>(type: "int", nullable: true),
                    RepaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reminder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanRepaymentPlanTypeRepaymentPlanTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepaymentMonthlySchedules", x => x.RepaymentPlanId);
                    table.ForeignKey(
                        name: "FK_LoanRepaymentMonthlySchedules_LoanApplication_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplication",
                        principalColumn: "LoanApplicationId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentMonthlySchedules_LoanRepaymentPlanType_LoanRepaymentPlanTypeRepaymentPlanTypeId",
                        column: x => x.LoanRepaymentPlanTypeRepaymentPlanTypeId,
                        principalTable: "LoanRepaymentPlanType",
                        principalColumn: "RepaymentPlanTypeId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentMonthlySchedules_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "LoanRepaymentPlan",
                columns: table => new
                {
                    LoanRepaymentPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepaymentPlanTypeId = table.Column<int>(type: "int", nullable: true),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstMonthlyRepayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMonthlyRepayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FirstRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecondRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThirdRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FourthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FifthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SixthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeventhRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EightRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NinethRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EleventhRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwelvethRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThirteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FourteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FifteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SixteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeventeenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EighteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NineteenthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwentiethRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwentyFirstRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwentySecondRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwentyThirdRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TwentyFourthRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumberofMonth = table.Column<int>(type: "int", nullable: true),
                    RepaymentDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RepaymentDuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    LoanRepaymentPlanTypeRepaymentPlanTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepaymentPlan", x => x.LoanRepaymentPlanId);
                    table.ForeignKey(
                        name: "FK_LoanRepaymentPlan_LoanApplication_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplication",
                        principalColumn: "LoanApplicationId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentPlan_LoanRepaymentPlanType_LoanRepaymentPlanTypeRepaymentPlanTypeId",
                        column: x => x.LoanRepaymentPlanTypeRepaymentPlanTypeId,
                        principalTable: "LoanRepaymentPlanType",
                        principalColumn: "RepaymentPlanTypeId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentPlan_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "LoanRepaymentSummary",
                columns: table => new
                {
                    LoanRepaymentSummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstRepaymentMonth = table.Column<int>(type: "int", nullable: true),
                    FirstRepaymentYear = table.Column<int>(type: "int", nullable: true),
                    LastRepaymentMonth = table.Column<int>(type: "int", nullable: true),
                    LastRepaymentYear = table.Column<int>(type: "int", nullable: true),
                    TotalLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalAmountRepaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NumberofMonthsPaid = table.Column<int>(type: "int", nullable: true),
                    LoanDurationInMonths = table.Column<int>(type: "int", nullable: true),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    LoanRepaymentPlanId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentPlanTypeId = table.Column<int>(type: "int", nullable: true),
                    ExpectedTotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepaymentSummary", x => x.LoanRepaymentSummaryId);
                    table.ForeignKey(
                        name: "FK_LoanRepaymentSummary_LoanApplication_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplication",
                        principalColumn: "LoanApplicationId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentSummary_LoanType_LoanTypeId",
                        column: x => x.LoanTypeId,
                        principalTable: "LoanType",
                        principalColumn: "LoanTypeId");
                    table.ForeignKey(
                        name: "FK_LoanRepaymentSummary_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberAccount",
                columns: table => new
                {
                    MemberAccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountTypeID = table.Column<int>(type: "int", nullable: true),
                    DateOpened = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitialDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByStaffID = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BVNNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAccount", x => x.MemberAccountID);
                    table.ForeignKey(
                        name: "FK_MemberAccount_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberBank",
                columns: table => new
                {
                    CBId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CBAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CBAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CBName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CBSortCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmploymentTypeId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberBank", x => x.CBId);
                    table.ForeignKey(
                        name: "FK_MemberBank_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberGuarantor",
                columns: table => new
                {
                    GuarantorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlyRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthlyProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberGuarantor", x => x.GuarantorID);
                    table.ForeignKey(
                        name: "FK_MemberGuarantor_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberSavingDefinition",
                columns: table => new
                {
                    MemberSavingDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuurentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSavingDefinition", x => x.MemberSavingDefinitionId);
                    table.ForeignKey(
                        name: "FK_MemberSavingDefinition_AccountType_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeID");
                    table.ForeignKey(
                        name: "FK_MemberSavingDefinition_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "tblReceipt",
                columns: table => new
                {
                    ReceiptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmountPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReceiptAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptStaffID = table.Column<int>(type: "int", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayProof = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TellerNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentRefCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgId = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblReceipt", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_tblReceipt_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "LoanRepayment",
                columns: table => new
                {
                    RepaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepaymentDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentPlanId = table.Column<int>(type: "int", nullable: true),
                    RepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepaymentAmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingBal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PenaltyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    DatePaid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnytPenalty = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepaymentCount = table.Column<int>(type: "int", nullable: true),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HODDateReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HODReviewedBy = table.Column<int>(type: "int", nullable: true),
                    HODReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    LoanRepaymentMonthlyScheduleRepaymentPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepayment", x => x.RepaymentId);
                    table.ForeignKey(
                        name: "FK_LoanRepayment_LoanApplication_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplication",
                        principalColumn: "LoanApplicationId");
                    table.ForeignKey(
                        name: "FK_LoanRepayment_LoanRepaymentMonthlySchedules_LoanRepaymentMonthlyScheduleRepaymentPlanId",
                        column: x => x.LoanRepaymentMonthlyScheduleRepaymentPlanId,
                        principalTable: "LoanRepaymentMonthlySchedules",
                        principalColumn: "RepaymentPlanId");
                    table.ForeignKey(
                        name: "FK_LoanRepayment_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberSavingSummary",
                columns: table => new
                {
                    SavingSummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalDepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstSavingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FirstSavingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSavingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastSavingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSavingSummary", x => x.SavingSummaryId);
                    table.ForeignKey(
                        name: "FK_MemberSavingSummary_MemberAccount_MemberAccountID",
                        column: x => x.MemberAccountID,
                        principalTable: "MemberAccount",
                        principalColumn: "MemberAccountID");
                    table.ForeignKey(
                        name: "FK_MemberSavingSummary_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberTransactionBalance",
                columns: table => new
                {
                    MemberTransactionBalanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTransactionBalance", x => x.MemberTransactionBalanceId);
                    table.ForeignKey(
                        name: "FK_MemberTransactionBalance_MemberAccount_MemberAccountID",
                        column: x => x.MemberAccountID,
                        principalTable: "MemberAccount",
                        principalColumn: "MemberAccountID");
                    table.ForeignKey(
                        name: "FK_MemberTransactionBalance_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberWithdrawal",
                columns: table => new
                {
                    MemberWithdrawalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WithdrawalDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    WithdrawalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WithdrawalRequestedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminFeeCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithdrawalType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HODDateReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HODReviewedBy = table.Column<int>(type: "int", nullable: true),
                    HODReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberWithdrawal", x => x.MemberWithdrawalID);
                    table.ForeignKey(
                        name: "FK_MemberWithdrawal_MemberAccount_MemberAccountID",
                        column: x => x.MemberAccountID,
                        principalTable: "MemberAccount",
                        principalColumn: "MemberAccountID");
                    table.ForeignKey(
                        name: "FK_MemberWithdrawal_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "MemberSaving_Deposit",
                columns: table => new
                {
                    MemberSavingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberSavingDefinitionId = table.Column<int>(type: "int", nullable: true),
                    MemberAccountID = table.Column<int>(type: "int", nullable: true),
                    MemberNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SavingDay = table.Column<int>(type: "int", nullable: true),
                    SavingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SavingMonth = table.Column<int>(type: "int", nullable: true),
                    SavingYear = table.Column<int>(type: "int", nullable: true),
                    AmountDeposited = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConfirmationStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedBy = table.Column<int>(type: "int", nullable: true),
                    ConfirmDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: true),
                    ReferenceUniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HODDateReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HODReviewedBy = table.Column<int>(type: "int", nullable: true),
                    HODReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: true),
                    DepositTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSaving_Deposit", x => x.MemberSavingId);
                    table.ForeignKey(
                        name: "FK_MemberSaving_Deposit_AccountType_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeID");
                    table.ForeignKey(
                        name: "FK_MemberSaving_Deposit_MemberAccount_MemberAccountID",
                        column: x => x.MemberAccountID,
                        principalTable: "MemberAccount",
                        principalColumn: "MemberAccountID");
                    table.ForeignKey(
                        name: "FK_MemberSaving_Deposit_MemberSavingDefinition_MemberSavingDefinitionId",
                        column: x => x.MemberSavingDefinitionId,
                        principalTable: "MemberSavingDefinition",
                        principalColumn: "MemberSavingDefinitionId");
                    table.ForeignKey(
                        name: "FK_MemberSaving_Deposit_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountType_StaffID",
                table: "AccountType",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_audittrailHistory_audittrailid",
                table: "audittrailHistory",
                column: "audittrailid");

            migrationBuilder.CreateIndex(
                name: "IX_BirthDayCerebration_StaffID",
                table: "BirthDayCerebration",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_CoorperatorBooklet_ContributionTypeId",
                table: "CoorperatorBooklet",
                column: "ContributionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoorperatorBooklet_DeductionTypeId",
                table: "CoorperatorBooklet",
                column: "DeductionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoorperatorBooklet_MemberId",
                table: "CoorperatorBooklet",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplication_LoanTypeId",
                table: "LoanApplication",
                column: "LoanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplication_StaffID",
                table: "LoanApplication",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayment_LoanApplicationId",
                table: "LoanRepayment",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayment_LoanRepaymentMonthlyScheduleRepaymentPlanId",
                table: "LoanRepayment",
                column: "LoanRepaymentMonthlyScheduleRepaymentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayment_MemberId",
                table: "LoanRepayment",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentMonthlySchedules_LoanApplicationId",
                table: "LoanRepaymentMonthlySchedules",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentMonthlySchedules_LoanRepaymentPlanTypeRepaymentPlanTypeId",
                table: "LoanRepaymentMonthlySchedules",
                column: "LoanRepaymentPlanTypeRepaymentPlanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentMonthlySchedules_MemberId",
                table: "LoanRepaymentMonthlySchedules",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentPlan_LoanApplicationId",
                table: "LoanRepaymentPlan",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentPlan_LoanRepaymentPlanTypeRepaymentPlanTypeId",
                table: "LoanRepaymentPlan",
                column: "LoanRepaymentPlanTypeRepaymentPlanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentPlan_MemberId",
                table: "LoanRepaymentPlan",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentSummary_LoanApplicationId",
                table: "LoanRepaymentSummary",
                column: "LoanApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentSummary_LoanTypeId",
                table: "LoanRepaymentSummary",
                column: "LoanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepaymentSummary_MemberId",
                table: "LoanRepaymentSummary",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_RoleId",
                table: "Member",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_StaffID",
                table: "Member",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAccount_MemberId",
                table: "MemberAccount",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberBank_MemberId",
                table: "MemberBank",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberGuarantor_MemberId",
                table: "MemberGuarantor",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSaving_Deposit_AccountTypeId",
                table: "MemberSaving_Deposit",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSaving_Deposit_MemberAccountID",
                table: "MemberSaving_Deposit",
                column: "MemberAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSaving_Deposit_MemberId",
                table: "MemberSaving_Deposit",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSaving_Deposit_MemberSavingDefinitionId",
                table: "MemberSaving_Deposit",
                column: "MemberSavingDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSavingDefinition_AccountTypeId",
                table: "MemberSavingDefinition",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSavingDefinition_MemberId",
                table: "MemberSavingDefinition",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSavingSummary_MemberAccountID",
                table: "MemberSavingSummary",
                column: "MemberAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSavingSummary_MemberId",
                table: "MemberSavingSummary",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTransactionBalance_MemberAccountID",
                table: "MemberTransactionBalance",
                column: "MemberAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTransactionBalance_MemberId",
                table: "MemberTransactionBalance",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberWithdrawal_MemberAccountID",
                table: "MemberWithdrawal",
                column: "MemberAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberWithdrawal_MemberId",
                table: "MemberWithdrawal",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPass_StaffID",
                table: "ResetPass",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_RoleId",
                table: "Staff",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_Sal_StaffID",
                table: "Staff_Sal",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffSalaryDefinition_StaffID",
                table: "StaffSalaryDefinition",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicketReponses_SupportTickettTicketId",
                table: "SupportTicketReponses",
                column: "SupportTickettTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_tblAsset_StaffID",
                table: "tblAsset",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_tblExpenses_StaffID",
                table: "tblExpenses",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_tblExpenses_tblExpensesCategoryExpensesCategoryID",
                table: "tblExpenses",
                column: "tblExpensesCategoryExpensesCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_tblExpenses_tblExpensesTypeExpensesTypeID",
                table: "tblExpenses",
                column: "tblExpensesTypeExpensesTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_tblLGA_tblStateStateId",
                table: "tblLGA",
                column: "tblStateStateId");

            migrationBuilder.CreateIndex(
                name: "IX_tblReceipt_MemberId",
                table: "tblReceipt",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_tblSessionLog_StaffID",
                table: "tblSessionLog",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string proc_1 = "DROP PROCEDURE [dbo].[AuditTrailProc]";

            string proc_2 = "DROP PROCEDURE [dbo].[ProcAutoFlagOverDuePayment]";

            migrationBuilder.Sql(proc_1);
            migrationBuilder.Sql(proc_2);

            migrationBuilder.DropTable(
                name: "audittrailHistory");

            migrationBuilder.DropTable(
                name: "BillingType");

            migrationBuilder.DropTable(
                name: "BirthDayCerebration");

            migrationBuilder.DropTable(
                name: "CompanyAccountDetail");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "CoorperatorBooklet");

            migrationBuilder.DropTable(
                name: "Delegation");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "EmailMessage");

            migrationBuilder.DropTable(
                name: "LeadPosition");

            migrationBuilder.DropTable(
                name: "LoanPenalty");

            migrationBuilder.DropTable(
                name: "LoanRepayment");

            migrationBuilder.DropTable(
                name: "LoanRepaymentPlan");

            migrationBuilder.DropTable(
                name: "LoanRepaymentSummary");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "MemberBank");

            migrationBuilder.DropTable(
                name: "MemberGuarantor");

            migrationBuilder.DropTable(
                name: "MemberSaving_Deposit");

            migrationBuilder.DropTable(
                name: "MemberSavingSummary");

            migrationBuilder.DropTable(
                name: "MemberTransactionBalance");

            migrationBuilder.DropTable(
                name: "MemberWithdrawal");

            migrationBuilder.DropTable(
                name: "NetworkInfo");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "RegistrationFee");

            migrationBuilder.DropTable(
                name: "ResetPass");

            migrationBuilder.DropTable(
                name: "Staff_Sal");

            migrationBuilder.DropTable(
                name: "StaffCategory");

            migrationBuilder.DropTable(
                name: "StaffPension");

            migrationBuilder.DropTable(
                name: "StaffSalaryDefinition");

            migrationBuilder.DropTable(
                name: "StaffTax");

            migrationBuilder.DropTable(
                name: "SupportTicketReponses");

            migrationBuilder.DropTable(
                name: "tblAdministrativeCharge");

            migrationBuilder.DropTable(
                name: "tblAsset");

            migrationBuilder.DropTable(
                name: "tblaudittrail");

            migrationBuilder.DropTable(
                name: "tblAuditTrailMod");

            migrationBuilder.DropTable(
                name: "tblBank");

            migrationBuilder.DropTable(
                name: "tblExpenses");

            migrationBuilder.DropTable(
                name: "tblGeneralJournal");

            migrationBuilder.DropTable(
                name: "tblInterestOnAccount");

            migrationBuilder.DropTable(
                name: "tblLedger");

            migrationBuilder.DropTable(
                name: "tblLGA");

            migrationBuilder.DropTable(
                name: "tblNotificationMethod");

            migrationBuilder.DropTable(
                name: "tblOTPHistory");

            migrationBuilder.DropTable(
                name: "tblReceipt");

            migrationBuilder.DropTable(
                name: "tblSessionLog");

            migrationBuilder.DropTable(
                name: "tblShareHoldersEquity");

            migrationBuilder.DropTable(
                name: "tblSMSMessage");

            migrationBuilder.DropTable(
                name: "tblSMSNetworkInfo");

            migrationBuilder.DropTable(
                name: "tempLoanCalculationtable");

            migrationBuilder.DropTable(
                name: "temptblBalanceSheet");

            migrationBuilder.DropTable(
                name: "temptblIncomeandExpenditures");

            migrationBuilder.DropTable(
                name: "UserDelegation");

            migrationBuilder.DropTable(
                name: "UserReferralTracking");

            migrationBuilder.DropTable(
                name: "WFStatus");

            migrationBuilder.DropTable(
                name: "audittrail");

            migrationBuilder.DropTable(
                name: "ContributionType");

            migrationBuilder.DropTable(
                name: "DeductionType");

            migrationBuilder.DropTable(
                name: "LoanRepaymentMonthlySchedules");

            migrationBuilder.DropTable(
                name: "MemberSavingDefinition");

            migrationBuilder.DropTable(
                name: "MemberAccount");

            migrationBuilder.DropTable(
                name: "SupportTicketts");

            migrationBuilder.DropTable(
                name: "tblExpensesCategory");

            migrationBuilder.DropTable(
                name: "tblExpensesType");

            migrationBuilder.DropTable(
                name: "tblState");

            migrationBuilder.DropTable(
                name: "LoanApplication");

            migrationBuilder.DropTable(
                name: "LoanRepaymentPlanType");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "LoanType");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
