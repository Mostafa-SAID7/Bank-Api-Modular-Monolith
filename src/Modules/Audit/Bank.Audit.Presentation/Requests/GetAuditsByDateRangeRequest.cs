namespace Bank.Audit.Presentation.Requests;

public record GetAuditsByDateRangeRequest(DateTime StartDate, DateTime EndDate);
