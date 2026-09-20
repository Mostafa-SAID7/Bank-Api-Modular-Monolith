using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Bank.Loans.Infrastructure.Data;

#nullable disable

namespace Bank.Loans.Infrastructure.Data.Migrations
{
    [DbContext(typeof(LoansDbContext))]
    partial class LoansDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("loans")
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Bank.Loans.Domain.Entities.Loan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal?>("ApprovedAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<Guid?>("ApprovedByUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("ApprovalNotes")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<DateTime>("AppliedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CancellationReason")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime?>("CancelledDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CollateralDescription")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<decimal?>("CollateralValue")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DefaultedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DefaultReason")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("DisbursementReferenceNumber")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("DisbursedDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EMIFrequency")
                        .HasColumnType("integer");

                    b.Property<int>("InterestType")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastPaymentDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("LoanAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<string>("LoanNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid>("LoanProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("LoanType")
                        .HasColumnType("integer");

                    b.Property<decimal?>("MonthlyEMI")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime?>("NextEMIDueDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal?>("OutstandingInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal?>("OutstandingPrincipal")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("PaidInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("PaidPrincipal")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal?>("PrepaymentPenaltyPercent")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric(5,2)");

                    b.Property<string>("RejectionReason")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<decimal>("RateOfInterest")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric(5,2)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("TenureMonths")
                        .HasColumnType("integer");

                    b.Property<decimal?>("TotalAmountPayable")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal?>("TotalInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AppliedAtUtc");

                    b.HasIndex("CustomerId");

                    b.HasIndex("LoanNumber")
                        .IsUnique();

                    b.HasIndex("LoanType");

                    b.HasIndex("NextEMIDueDateUtc");

                    b.HasIndex("Status");

                    b.ToTable("loans", "loans");
                });

            modelBuilder.Entity("Bank.Loans.Domain.Entities.LoanProduct", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("BaseRateOfInterest")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric(5,2)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<decimal?>("DocumentationFee")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsCollateralRequired")
                        .HasColumnType("boolean");

                    b.Property<int>("LoanType")
                        .HasColumnType("integer");

                    b.Property<decimal>("MaxCollateralValue")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("MaxLoanAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<int>("MaxTenureMonths")
                        .HasColumnType("integer");

                    b.Property<decimal>("MinCollateralValue")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("MinLoanAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<int>("MinTenureMonths")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal?>("ProcessingFeePercent")
                        .HasPrecision(5, 2)
                        .HasColumnType("numeric(5,2)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IsActive");

                    b.HasIndex("LoanType");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("loan_products", "loans");
                });

            modelBuilder.Entity("Bank.Loans.Domain.Entities.LoanSchedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DueDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("EMIAmount")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("InterestComponent")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.Property<Guid>("LoanId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("PaidInterest")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime?>("PaidDateUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("PaidPrincipal")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("PrincipalComponent")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<decimal>("RemainingPrincipal")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)");

                    b.Property<int>("ScheduleMonth")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DueDateUtc");

                    b.HasIndex("IsPaid");

                    b.HasIndex("LoanId");

                    b.ToTable("loan_schedules", "loans");
                });
#pragma warning restore 612, 618
        }
    }
}
