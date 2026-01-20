namespace Expenses.API.DTOs;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<ExpenseTypeDto> Expenses { get; set; } = new();
    public string? PreferredCurrency { get; set; }
    public decimal? MonthlySalary { get; set; }
    public string? SalaryInputCurrency { get; set; } // Moneda en que se ingresó el salario originalmente
    public decimal? IdealSavings { get; set; }
    public string? SavingsInputCurrency { get; set; } // Moneda en que se ingresaron los ahorros originalmente
    public decimal? IdealMonthlyExpenses { get; set; }
    public decimal? ExchangeRateBuy { get; set; }
    public decimal? ExchangeRateSell { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
