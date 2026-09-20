using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Bank.Deposits.Infrastructure.Data.Migrations
{
    [DbContext(typeof(DepositsDbContext))]
    partial class DepositsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasDefaultSchema("deposits");

            modelBuilder.Entity("Bank.Deposits.Domain.Entities.Deposit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("text");

                    b.Property<decimal>("AccruedInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("ClosedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ClosureReason")
                        .HasMaxLength(500)
                        .HasColumnType("text");

                    b.Property<decimal>("CurrentBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DepositTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("FreezeReason")
                        .HasMaxLength(500)
                        .HasColumnType("text");

                    b.Property<int?>("InterestFrequency")
                        .HasColumnType("integer");

                    b.Property<bool>("IsFixedDeposit")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastInterestAccrualUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("MaturityDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("NextInterestPaymentUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("OpenedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("PaidInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int?>("TermMonths")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AccountNumber")
                        .IsUnique();

                    b.HasIndex("CustomerId");

                    b.HasIndex("MaturityDateUtc");

                    b.HasIndex("OpenedAtUtc");

                    b.HasIndex("Status");

                    b.ToTable("deposits", "deposits");
                });

            modelBuilder.Entity("Bank.Deposits.Domain.Entities.DepositType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AllowsEarlyWithdrawal")
                        .HasColumnType("boolean");

                    b.Property<bool>("AllowsPrematureClosure")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("text");

                    b.Property<int?>("DefaultTermMonths")
                        .HasColumnType("integer");

                    b.Property<decimal?>("EarlyWithdrawalPenaltyPercent")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsFixedDeposit")
                        .HasColumnType("boolean");

                    b.Property<decimal>("MaximumBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<decimal>("MinimumBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("text");

                    b.Property<decimal?>("PrematureClosurePenaltyPercent")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("deposit_types", "deposits");
                });

            modelBuilder.Entity("Bank.Deposits.Domain.Entities.InterestRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CalculationMethod")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DepositTypeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("EffectiveToUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EffectiveFromUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Frequency")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<decimal?>("MaximumBalanceForRate")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<decimal?>("MinimumBalanceForRate")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric");

                    b.Property<decimal>("AnnualRate")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DepositTypeId");

                    b.HasIndex("EffectiveFromUtc");

                    b.HasIndex("IsActive");

                    b.ToTable("interest_rates", "deposits");
                });
#pragma warning restore 612, 618
        }
    }
}
