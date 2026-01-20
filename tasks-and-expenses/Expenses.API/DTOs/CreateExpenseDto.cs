using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Expenses.API.DTOs;

public class CreateExpenseDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public List<ExpenseTypeDto> Expenses { get; set; } = new();
    
    [MaxLength(3)]
    public string? PreferredCurrency { get; set; }
    
    public decimal? MonthlySalary { get; set; }
    
    [MaxLength(3)]
    public string? SalaryInputCurrency { get; set; } // Moneda en que se ingresó el salario originalmente
    
    public decimal? IdealSavings { get; set; }
    
    [MaxLength(3)]
    public string? SavingsInputCurrency { get; set; } // Moneda en que se ingresaron los ahorros originalmente
    
    public decimal? IdealMonthlyExpenses { get; set; }
    
    public decimal? ExchangeRateBuy { get; set; }
    
    public decimal? ExchangeRateSell { get; set; }
    
    public int? Month { get; set; } // Mes (1-12), si es null se usa el mes actual
    
    public int? Year { get; set; } // Año, si es null se usa el año actual
}

public class ExpenseTypeDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue, ErrorMessage = "El límite debe ser mayor o igual a 0")]
    public decimal Limit { get; set; } = 0; // Gasto esperado/estimado
    
    [Range(0, double.MaxValue, ErrorMessage = "El gasto real debe ser mayor o igual a 0")]
    public decimal? ActualAmount { get; set; } = null; // Gasto real
    
    [MaxLength(3)]
    public string Currency { get; set; } = "USD"; // Moneda del gasto individual
    
    // Campos editables por moneda - estos son los que el frontend necesita poder editar
    [JsonPropertyName("expectedUSD")]
    public decimal? ExpectedUSD { get; set; }
    
    [JsonPropertyName("actualUSD")]
    public decimal? ActualUSD { get; set; }
    
    [JsonPropertyName("expectedCRC")]
    public decimal? ExpectedCRC { get; set; }
    
    [JsonPropertyName("actualCRC")]
    public decimal? ActualCRC { get; set; }
    
    // También aceptar campos con espacios (como vienen del frontend)
    [System.Text.Json.Serialization.JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; set; }
    
    [MaxLength(20)]
    public string? PaymentMethod { get; set; } = null; // "credit" o "debit" - método de pago
    
    public int? CreditCardId { get; set; } = null; // ID de la tarjeta de crédito si aplica (para futuras fechas de corte/pago)
}
