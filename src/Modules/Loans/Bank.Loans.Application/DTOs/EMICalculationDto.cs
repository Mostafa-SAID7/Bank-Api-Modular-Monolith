namespace Bank.Loans.Application.DTOs;

public record EMICalculationDto(
    decimal Principal,
    decimal AnnualRate,
    int TenureMonths,
    decimal MonthlyEMI,
    decimal TotalAmount,
    decimal TotalInterest);
