using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Deposits.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFixedDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create deposit_products table
            migrationBuilder.CreateTable(
                name: "deposit_products",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MinimumAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MinimumTermMonths = table.Column<int>(type: "integer", nullable: false),
                    MaximumTermMonths = table.Column<int>(type: "integer", nullable: false),
                    BaseInterestRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    InterestFrequency = table.Column<int>(type: "integer", nullable: false),
                    InterestCalculationMethod = table.Column<int>(type: "integer", nullable: false),
                    AllowsEarlyWithdrawal = table.Column<bool>(type: "boolean", nullable: false),
                    WithdrawalPenaltyType = table.Column<int>(type: "integer", nullable: false),
                    WithdrawalPenaltyPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit_products", x => x.Id);
                });

            // Create fixed_deposits table
            migrationBuilder.CreateTable(
                name: "fixed_deposits",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepositNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepositProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    TermDays = table.Column<int>(type: "integer", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaturityDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InterestCalculationMethod = table.Column<int>(type: "integer", nullable: false),
                    CompoundingFrequency = table.Column<int>(type: "integer", nullable: false),
                    AccruedInterest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LastInterestCalculationDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaturityAction = table.Column<int>(type: "integer", nullable: false),
                    AutoRenewalEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RenewalTermDays = table.Column<int>(type: "integer", nullable: true),
                    RenewalNoticeDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerConsentReceived = table.Column<bool>(type: "boolean", nullable: false),
                    PenaltyType = table.Column<int>(type: "integer", nullable: false),
                    PenaltyAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PenaltyPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ClosureDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClosureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PenaltyApplied = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NetAmountPaid = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    RenewedFromDepositId = table.Column<Guid>(type: "uuid", nullable: true),
                    RenewedToDepositId = table.Column<Guid>(type: "uuid", nullable: true),
                    RenewalCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fixed_deposits", x => x.Id);
                });

            // Create deposit_transactions table
            migrationBuilder.CreateTable(
                name: "deposit_transactions",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FixedDepositId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Penalty = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit_transactions", x => x.Id);
                });

            // Create maturity_notices table
            migrationBuilder.CreateTable(
                name: "maturity_notices",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FixedDepositId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FixedDepositNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaturityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificationSent = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationSentAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerChoiceProvided = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerChoiceProvidedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerChoice = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maturity_notices", x => x.Id);
                });

            // Create indexes for deposit_products
            migrationBuilder.CreateIndex(
                name: "IX_deposit_products_IsActive",
                schema: "deposits",
                table: "deposit_products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_products_Name",
                schema: "deposits",
                table: "deposit_products",
                column: "Name",
                unique: true);

            // Create indexes for fixed_deposits
            migrationBuilder.CreateIndex(
                name: "IX_fixed_deposits_CreatedAtUtc",
                schema: "deposits",
                table: "fixed_deposits",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_fixed_deposits_CustomerId",
                schema: "deposits",
                table: "fixed_deposits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_fixed_deposits_DepositNumber",
                schema: "deposits",
                table: "fixed_deposits",
                column: "DepositNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fixed_deposits_MaturityDateUtc",
                schema: "deposits",
                table: "fixed_deposits",
                column: "MaturityDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_fixed_deposits_Status",
                schema: "deposits",
                table: "fixed_deposits",
                column: "Status");

            // Create indexes for deposit_transactions
            migrationBuilder.CreateIndex(
                name: "IX_deposit_transactions_FixedDepositId",
                schema: "deposits",
                table: "deposit_transactions",
                column: "FixedDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_transactions_TransactionDateUtc",
                schema: "deposits",
                table: "deposit_transactions",
                column: "TransactionDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_transactions_TransactionType",
                schema: "deposits",
                table: "deposit_transactions",
                column: "TransactionType");

            // Create indexes for maturity_notices
            migrationBuilder.CreateIndex(
                name: "IX_maturity_notices_CustomerId",
                schema: "deposits",
                table: "maturity_notices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_maturity_notices_FixedDepositId",
                schema: "deposits",
                table: "maturity_notices",
                column: "FixedDepositId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_maturity_notices_MaturityDate",
                schema: "deposits",
                table: "maturity_notices",
                column: "MaturityDate");

            migrationBuilder.CreateIndex(
                name: "IX_maturity_notices_NotificationSent",
                schema: "deposits",
                table: "maturity_notices",
                column: "NotificationSent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maturity_notices",
                schema: "deposits");

            migrationBuilder.DropTable(
                name: "deposit_transactions",
                schema: "deposits");

            migrationBuilder.DropTable(
                name: "fixed_deposits",
                schema: "deposits");

            migrationBuilder.DropTable(
                name: "deposit_products",
                schema: "deposits");
        }
    }
}
