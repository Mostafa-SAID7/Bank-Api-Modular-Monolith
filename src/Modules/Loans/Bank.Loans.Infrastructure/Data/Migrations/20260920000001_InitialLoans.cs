using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Loans.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "loans");

            migrationBuilder.CreateTable(
                name: "loan_products",
                schema: "loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LoanType = table.Column<int>(type: "integer", nullable: false),
                    MinLoanAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxLoanAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MinTenureMonths = table.Column<int>(type: "integer", nullable: false),
                    MaxTenureMonths = table.Column<int>(type: "integer", nullable: false),
                    BaseRateOfInterest = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ProcessingFeePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    DocumentationFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsCollateralRequired = table.Column<bool>(type: "boolean", nullable: false),
                    MinCollateralValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxCollateralValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loan_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                schema: "loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TenureMonths = table.Column<int>(type: "integer", nullable: false),
                    RateOfInterest = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MonthlyEMI = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalInterest = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalAmountPayable = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OutstandingPrincipal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    OutstandingInterest = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PaidPrincipal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidInterest = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LoanType = table.Column<int>(type: "integer", nullable: false),
                    InterestType = table.Column<int>(type: "integer", nullable: false),
                    EMIFrequency = table.Column<int>(type: "integer", nullable: false),
                    CollateralValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CollateralDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisbursedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisbursementReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NextEMIDueDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPaymentDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DefaultedDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DefaultReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CancelledDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PrepaymentPenaltyPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    AppliedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "loan_schedules",
                schema: "loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleMonth = table.Column<int>(type: "integer", nullable: false),
                    DueDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EMIAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PrincipalComponent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestComponent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingPrincipal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidPrincipal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidInterest = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loan_schedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loan_products_IsActive",
                schema: "loans",
                table: "loan_products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_loan_products_LoanType",
                schema: "loans",
                table: "loan_products",
                column: "LoanType");

            migrationBuilder.CreateIndex(
                name: "IX_loan_products_Name",
                schema: "loans",
                table: "loan_products",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loans_AppliedAtUtc",
                schema: "loans",
                table: "loans",
                column: "AppliedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_loans_CustomerId",
                schema: "loans",
                table: "loans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_loans_LoanNumber",
                schema: "loans",
                table: "loans",
                column: "LoanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loans_LoanType",
                schema: "loans",
                table: "loans",
                column: "LoanType");

            migrationBuilder.CreateIndex(
                name: "IX_loans_NextEMIDueDateUtc",
                schema: "loans",
                table: "loans",
                column: "NextEMIDueDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_loans_Status",
                schema: "loans",
                table: "loans",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_loan_schedules_DueDateUtc",
                schema: "loans",
                table: "loan_schedules",
                column: "DueDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_loan_schedules_IsPaid",
                schema: "loans",
                table: "loan_schedules",
                column: "IsPaid");

            migrationBuilder.CreateIndex(
                name: "IX_loan_schedules_LoanId",
                schema: "loans",
                table: "loan_schedules",
                column: "LoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loan_schedules",
                schema: "loans");

            migrationBuilder.DropTable(
                name: "loans",
                schema: "loans");

            migrationBuilder.DropTable(
                name: "loan_products",
                schema: "loans");
        }
    }
}
