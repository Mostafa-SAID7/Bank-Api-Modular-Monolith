using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Deposits.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDeposits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "deposits");

            migrationBuilder.CreateTable(
                name: "deposit_types",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MinimumBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaximumBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    IsFixedDeposit = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultTermMonths = table.Column<int>(type: "integer", nullable: true),
                    AllowsEarlyWithdrawal = table.Column<bool>(type: "boolean", nullable: false),
                    EarlyWithdrawalPenaltyPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    AllowsPrematureClosure = table.Column<bool>(type: "boolean", nullable: false),
                    PrematureClosurePenaltyPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposit_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "interest_rates",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepositTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnualRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    EffectiveFromUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveToUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinimumBalanceForRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaximumBalanceForRate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    CalculationMethod = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_interest_rates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "deposits",
                schema: "deposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepositTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AccruedInterest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidInterest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaturityDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastInterestAccrualUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextInterestPaymentUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsFixedDeposit = table.Column<bool>(type: "boolean", nullable: false),
                    TermMonths = table.Column<int>(type: "integer", nullable: true),
                    InterestFrequency = table.Column<int>(type: "integer", nullable: true),
                    FreezeReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClosureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deposit_types_IsActive",
                schema: "deposits",
                table: "deposit_types",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_types_Name",
                schema: "deposits",
                table: "deposit_types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_interest_rates_DepositTypeId",
                schema: "deposits",
                table: "interest_rates",
                column: "DepositTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_interest_rates_EffectiveFromUtc",
                schema: "deposits",
                table: "interest_rates",
                column: "EffectiveFromUtc");

            migrationBuilder.CreateIndex(
                name: "IX_interest_rates_IsActive",
                schema: "deposits",
                table: "interest_rates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_AccountNumber",
                schema: "deposits",
                table: "deposits",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deposits_CustomerId",
                schema: "deposits",
                table: "deposits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_MaturityDateUtc",
                schema: "deposits",
                table: "deposits",
                column: "MaturityDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_OpenedAtUtc",
                schema: "deposits",
                table: "deposits",
                column: "OpenedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_Status",
                schema: "deposits",
                table: "deposits",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposits",
                schema: "deposits");

            migrationBuilder.DropTable(
                name: "interest_rates",
                schema: "deposits");

            migrationBuilder.DropTable(
                name: "deposit_types",
                schema: "deposits");
        }
    }
}
