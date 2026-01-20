using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expenses.API.Models;

public class Expense
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string Expenses { get; set; } = "[]";
    
    [MaxLength(3)]
    public string? PreferredCurrency { get; set; }
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal? MonthlySalary { get; set; }
    
    [MaxLength(3)]
    public string? SalaryInputCurrency { get; set; } // Moneda en que se ingresó el salario originalmente
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal? IdealSavings { get; set; }
    
    [MaxLength(3)]
    public string? SavingsInputCurrency { get; set; } // Moneda en que se ingresaron los ahorros originalmente
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal? IdealMonthlyExpenses { get; set; }
    
    [Column(TypeName = "decimal(12,4)")]
    public decimal? ExchangeRateBuy { get; set; } // Tipo de cambio de compra (cuando vendes USD)
    
    [Column(TypeName = "decimal(12,4)")]
    public decimal? ExchangeRateSell { get; set; } // Tipo de cambio de venta (cuando compras USD)
    
    [Required]
    public int Month { get; set; } // Mes (1-12)
    
    [Required]
    public int Year { get; set; } // Año (ej: 2024)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
