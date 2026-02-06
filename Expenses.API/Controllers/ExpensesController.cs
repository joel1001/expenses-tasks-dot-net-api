using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expenses.API.Data;
using Expenses.API.Models;
using Expenses.API.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Expenses.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly ExpensesDbContext _context;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(ExpensesDbContext context, ILogger<ExpensesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
    {
        var expenses = await _context.Expenses.ToListAsync();
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        var dtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            UserId = e.UserId,
            Expenses = JsonSerializer.Deserialize<List<ExpenseTypeDto>>(e.Expenses, jsonOptions) ?? new(),
            PreferredCurrency = e.PreferredCurrency,
            MonthlySalary = e.MonthlySalary,
            SalaryInputCurrency = e.SalaryInputCurrency,
            IdealSavings = e.IdealSavings,
            SavingsInputCurrency = e.SavingsInputCurrency,
            IdealMonthlyExpenses = e.IdealMonthlyExpenses,
            ExchangeRateBuy = e.ExchangeRateBuy,
            ExchangeRateSell = e.ExchangeRateSell,
            Month = e.Month,
            Year = e.Year,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToList();

        return dtos;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpense(Guid id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            return NotFound();
        }

        // Parsear el JSON de expenses
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        var expenseTypes = JsonSerializer.Deserialize<List<ExpenseTypeDto>>(expense.Expenses, jsonOptions) ?? new();
        
        var dto = new ExpenseDto
        {
            Id = expense.Id,
            UserId = expense.UserId,
            Expenses = expenseTypes,
            PreferredCurrency = expense.PreferredCurrency,
            MonthlySalary = expense.MonthlySalary,
            SalaryInputCurrency = expense.SalaryInputCurrency,
            IdealSavings = expense.IdealSavings,
            SavingsInputCurrency = expense.SavingsInputCurrency,
            IdealMonthlyExpenses = expense.IdealMonthlyExpenses,
            ExchangeRateBuy = expense.ExchangeRateBuy,
            ExchangeRateSell = expense.ExchangeRateSell,
            Month = expense.Month,
            Year = expense.Year,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };

        return dto;
    }

    /// <summary>
    /// Obtiene los gastos del usuario. Solo se muestran tablas de los últimos 2 años;
    /// cualquier documento más viejo se elimina de la base de datos.
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesByUser(Guid userId)
    {
        var cutoffYear = DateTime.UtcNow.Year - 2;

        var oldExpenses = await _context.Expenses
            .Where(e => e.UserId == userId && e.Year < cutoffYear)
            .ToListAsync();
        if (oldExpenses.Any())
        {
            _context.Expenses.RemoveRange(oldExpenses);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Removed {Count} expense document(s) older than year {CutoffYear} for user {UserId}",
                oldExpenses.Count, cutoffYear, userId);
        }

        // Orden natural: año ascendente, mes ascendente (Enero a Diciembre)
        var expenses = await _context.Expenses
            .Where(e => e.UserId == userId && e.Year >= cutoffYear)
            .OrderBy(e => e.Year)
            .ThenBy(e => e.Month)
            .ToListAsync();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var dtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            UserId = e.UserId,
            Expenses = JsonSerializer.Deserialize<List<ExpenseTypeDto>>(e.Expenses, jsonOptions) ?? new(),
            PreferredCurrency = e.PreferredCurrency,
            MonthlySalary = e.MonthlySalary,
            SalaryInputCurrency = e.SalaryInputCurrency,
            IdealSavings = e.IdealSavings,
            SavingsInputCurrency = e.SavingsInputCurrency,
            IdealMonthlyExpenses = e.IdealMonthlyExpenses,
            ExchangeRateBuy = e.ExchangeRateBuy,
            ExchangeRateSell = e.ExchangeRateSell,
            Month = e.Month,
            Year = e.Year,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToList();

        return dtos;
    }

    /// <summary>
    /// Obtiene gastos por userId (int de Users API). Convierte a Guid estándar: 00000000-0000-0000-0000-{userId hex}
    /// </summary>
    [HttpGet("by-user-id/{userIdInt:int}")]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesByUserIdInt(int userIdInt)
    {
        var hexId = userIdInt.ToString("x").PadLeft(12, '0');
        var userGuid = Guid.Parse($"00000000-0000-0000-0000-{hexId}");
        return await GetExpensesByUser(userGuid);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto dto)
    {
        _logger.LogInformation("Received CreateExpense request. UserId: {UserId}, Expenses Count: {Count}, Month: {Month}, Year: {Year}", 
            dto?.UserId, dto?.Expenses?.Count ?? 0, dto?.Month, dto?.Year);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        // Validar que los gastos tengan formato correcto
        if (dto.Expenses == null || dto.Expenses.Count == 0)
        {
            _logger.LogWarning("No expenses provided in request");
            return BadRequest(new { error = "Debe proporcionar al menos un tipo de gasto" });
        }

        // Procesar los expenses para preservar todos los campos, incluyendo Expected USD, Actual USD, etc.
        var processedExpenses = ProcessExpenseTypes(dto.Expenses);

        // Validar cada tipo de gasto
        foreach (var expenseType in processedExpenses)
        {
            if (string.IsNullOrWhiteSpace(expenseType.Name))
            {
                _logger.LogWarning("Expense type with empty name found");
                return BadRequest(new { error = "Todos los tipos de gasto deben tener un nombre" });
            }
            
            // Validar método de pago si se especifica
            if (!string.IsNullOrEmpty(expenseType.PaymentMethod))
            {
                if (expenseType.PaymentMethod != "credit" && expenseType.PaymentMethod != "debit")
                {
                    _logger.LogWarning("Invalid payment method: {PaymentMethod}", expenseType.PaymentMethod);
                    return BadRequest(new { error = "El método de pago debe ser 'credit' o 'debit'" });
                }
                
                // Si es crédito, debe tener un creditCardId válido
                if (expenseType.PaymentMethod == "credit" && (!expenseType.CreditCardId.HasValue || expenseType.CreditCardId.Value <= 0))
                {
                    _logger.LogWarning("Credit card payment method specified but no valid creditCardId provided");
                    return BadRequest(new { error = "Debe seleccionar una tarjeta de crédito válida" });
                }
            }
        }

        // Convertir la lista de gastos a JSON (usar processedExpenses que incluye todos los campos)
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        var expensesJson = JsonSerializer.Serialize(processedExpenses, jsonOptions);
        _logger.LogInformation("Serialized expenses to JSON: {Json}", expensesJson);
        _logger.LogInformation("Expense types include PaymentMethod and CreditCardId: {Types}", 
            string.Join(", ", processedExpenses.Select(e => $"{e.Name}: PaymentMethod={e.PaymentMethod}, CreditCardId={e.CreditCardId}")));
        
        // Log de campos Expected/Actual USD/CRC
        foreach (var expType in processedExpenses)
        {
            if (expType.ExpectedUSD.HasValue || expType.ActualUSD.HasValue || expType.ExpectedCRC.HasValue || expType.ActualCRC.HasValue)
            {
                _logger.LogInformation("  - {Name}: ExpectedUSD={ExpectedUSD}, ActualUSD={ActualUSD}, ExpectedCRC={ExpectedCRC}, ActualCRC={ActualCRC}",
                    expType.Name, expType.ExpectedUSD, expType.ActualUSD, expType.ExpectedCRC, expType.ActualCRC);
            }
        }
        
        // Verificar que el JSON contiene paymentMethod
        if (expensesJson.Contains("paymentMethod", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("✅ JSON contiene paymentMethod");
        }
        else
        {
            _logger.LogWarning("⚠️ JSON NO contiene paymentMethod");
        }

        // Determinar mes y año (usar mes/año actual si no se proporciona)
        var now = DateTime.UtcNow;
        var month = dto.Month.HasValue ? dto.Month.Value : now.Month;
        var year = dto.Year.HasValue ? dto.Year.Value : now.Year;
        
        _logger.LogInformation("Using Month: {Month}, Year: {Year} (from DTO: Month={DtoMonth}, Year={DtoYear})", 
            month, year, dto.Month, dto.Year);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Expenses = expensesJson,
            PreferredCurrency = dto.PreferredCurrency,
            MonthlySalary = dto.MonthlySalary,
            SalaryInputCurrency = dto.SalaryInputCurrency,
            IdealSavings = dto.IdealSavings,
            SavingsInputCurrency = dto.SavingsInputCurrency,
            IdealMonthlyExpenses = dto.IdealMonthlyExpenses,
            ExchangeRateBuy = dto.ExchangeRateBuy,
            ExchangeRateSell = dto.ExchangeRateSell,
            Month = month,
            Year = year,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _logger.LogInformation("Created Expense entity with Month: {Month}, Year: {Year}", expense.Month, expense.Year);

        try
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Expense created successfully. Id: {Id}, UserId: {UserId}", expense.Id, expense.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving expense to database");
            return StatusCode(500, new { error = "Error al guardar el gasto en la base de datos", details = ex.Message });
        }

        // Convertir de vuelta a DTO para la respuesta (reutilizar jsonOptions ya definido arriba)
        var expenseTypes = JsonSerializer.Deserialize<List<ExpenseTypeDto>>(expensesJson, jsonOptions) ?? new();
        var response = new ExpenseDto
        {
            Id = expense.Id,
            UserId = expense.UserId,
            Expenses = expenseTypes,
            PreferredCurrency = expense.PreferredCurrency,
            MonthlySalary = expense.MonthlySalary,
            SalaryInputCurrency = expense.SalaryInputCurrency,
            IdealSavings = expense.IdealSavings,
            SavingsInputCurrency = expense.SavingsInputCurrency,
            IdealMonthlyExpenses = expense.IdealMonthlyExpenses,
            ExchangeRateBuy = expense.ExchangeRateBuy,
            ExchangeRateSell = expense.ExchangeRateSell,
            Month = expense.Month,
            Year = expense.Year,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };

        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, response);
    }

    /// <summary>
    /// Guarda todos los cambios de la tabla en un solo request.
    /// El body debe incluir la lista completa de expense types (Expense Type, Currency, Payment Method, Expected/Actual USD/CRC).
    /// Los valores numéricos se limitan en el servidor para evitar números enormes.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] CreateExpenseDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        // Validar que los gastos tengan formato correcto
        if (dto.Expenses == null || dto.Expenses.Count == 0)
        {
            return BadRequest(new { error = "Debe proporcionar al menos un tipo de gasto" });
        }

        // Validar cada tipo de gasto
        foreach (var expenseType in dto.Expenses)
        {
            if (string.IsNullOrWhiteSpace(expenseType.Name))
            {
                return BadRequest(new { error = "Todos los tipos de gasto deben tener un nombre" });
            }
            
            // Validar método de pago si se especifica (acepta "credit", "debit", "Debit Account", "Payment method", etc.)
            if (!string.IsNullOrEmpty(expenseType.PaymentMethod))
            {
                var pm = expenseType.PaymentMethod.Trim().ToLowerInvariant();
                var isCredit = pm == "credit" || pm.Contains("credit");
                var isDebit = pm == "debit" || pm.Contains("debit") || pm.Contains("account") || pm.Contains("payment");
                if (!isCredit && !isDebit)
                {
                    return BadRequest(new { error = "El método de pago debe ser 'credit' o 'debit' (o ej. 'Debit Account')" });
                }

                if (isCredit && (!expenseType.CreditCardId.HasValue || expenseType.CreditCardId.Value <= 0))
                {
                    return BadRequest(new { error = "Debe seleccionar una tarjeta de crédito válida" });
                }
            }
        }

        // Convertir la lista de gastos a JSON
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        _logger.LogInformation("UpdateExpense - Received {Count} expense types", dto.Expenses?.Count ?? 0);
        foreach (var expType in dto.Expenses ?? new List<ExpenseTypeDto>())
        {
            _logger.LogInformation("  - {Name}: Limit={Limit}, ActualAmount={ActualAmount}, Currency={Currency}, ExpectedUSD={ExpectedUSD}, ActualUSD={ActualUSD}, ExpectedCRC={ExpectedCRC}, ActualCRC={ActualCRC}, PaymentMethod={PaymentMethod}, CreditCardId={CreditCardId}", 
                expType.Name, expType.Limit, expType.ActualAmount, expType.Currency,
                expType.ExpectedUSD, expType.ActualUSD, expType.ExpectedCRC, expType.ActualCRC,
                expType.PaymentMethod, expType.CreditCardId);
            
            // Log de AdditionalProperties si existen (campos con espacios como "Expected USD")
            if (expType.AdditionalProperties != null && expType.AdditionalProperties.Count > 0)
            {
                _logger.LogInformation("  - {Name} AdditionalProperties: {Props}",
                    expType.Name, string.Join(", ", expType.AdditionalProperties.Select(kvp => $"{kvp.Key}={kvp.Value}")));
            }
        }
        
        // Procesar los expenses para preservar todos los campos, incluyendo los que vienen con espacios
        var processedExpenses = ProcessExpenseTypes(dto.Expenses ?? new List<ExpenseTypeDto>());
        
        var expensesJson = JsonSerializer.Serialize(processedExpenses, jsonOptions);
        _logger.LogInformation("UpdateExpense - Serialized JSON: {Json}", expensesJson);
        
        // Verificar que el JSON contiene los campos esperados
        var jsonLower = expensesJson.ToLowerInvariant();
        if (jsonLower.Contains("expectedusd") || jsonLower.Contains("actualusd") || 
            jsonLower.Contains("expectedcrc") || jsonLower.Contains("actualcrc") ||
            expensesJson.Contains("Expected USD", StringComparison.OrdinalIgnoreCase) ||
            expensesJson.Contains("Actual USD", StringComparison.OrdinalIgnoreCase) ||
            expensesJson.Contains("Expected CRC", StringComparison.OrdinalIgnoreCase) ||
            expensesJson.Contains("Actual CRC", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("✅ UpdateExpense - JSON contiene campos Expected/Actual USD/CRC");
        }
        
        if (expensesJson.Contains("paymentMethod", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("✅ UpdateExpense - JSON contiene paymentMethod");
        }

        // Determinar mes y año (usar mes/año actual si no se proporciona, o mantener el existente)
        var now = DateTime.UtcNow;
        var month = dto.Month.HasValue ? dto.Month.Value : (expense.Month > 0 ? expense.Month : now.Month);
        var year = dto.Year.HasValue ? dto.Year.Value : (expense.Year > 0 ? expense.Year : now.Year);
        
        _logger.LogInformation("UpdateExpense - Using Month: {Month}, Year: {Year} (from DTO: Month={DtoMonth}, Year={DtoYear}, existing: Month={ExistingMonth}, Year={ExistingYear})", 
            month, year, dto.Month, dto.Year, expense.Month, expense.Year);

        expense.UserId = dto.UserId;
        expense.Expenses = expensesJson;
        expense.PreferredCurrency = dto.PreferredCurrency;
        expense.MonthlySalary = dto.MonthlySalary;
        expense.SalaryInputCurrency = dto.SalaryInputCurrency;
        expense.IdealSavings = dto.IdealSavings;
        expense.SavingsInputCurrency = dto.SavingsInputCurrency;
        expense.IdealMonthlyExpenses = dto.IdealMonthlyExpenses;
        expense.ExchangeRateBuy = dto.ExchangeRateBuy;
        expense.ExchangeRateSell = dto.ExchangeRateSell;
        expense.Month = month;
        expense.Year = year;
        expense.UpdatedAt = DateTime.UtcNow;

        _context.Entry(expense).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExpenseExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private const decimal MaxSafeAmount = 999999999.99m;

    /// <summary>
    /// Procesa los ExpenseTypes: preserva campos, limita montos para evitar números enormes, normaliza PaymentMethod.
    /// </summary>
    private List<ExpenseTypeDto> ProcessExpenseTypes(List<ExpenseTypeDto> expenses)
    {
        var processed = new List<ExpenseTypeDto>();

        foreach (var expense in expenses)
        {
            var paymentMethod = NormalizePaymentMethod(expense.PaymentMethod);

            var processedExpense = new ExpenseTypeDto
            {
                Name = expense.Name,
                Limit = ClampDecimal(expense.Limit),
                ActualAmount = expense.ActualAmount.HasValue ? ClampDecimal(expense.ActualAmount.Value) : null,
                Currency = expense.Currency ?? "USD",
                ExpectedUSD = expense.ExpectedUSD.HasValue ? ClampDecimal(expense.ExpectedUSD.Value) : null,
                ActualUSD = expense.ActualUSD.HasValue ? ClampDecimal(expense.ActualUSD.Value) : null,
                ExpectedCRC = expense.ExpectedCRC.HasValue ? ClampDecimal(expense.ExpectedCRC.Value) : null,
                ActualCRC = expense.ActualCRC.HasValue ? ClampDecimal(expense.ActualCRC.Value) : null,
                PaymentMethod = paymentMethod,
                CreditCardId = expense.CreditCardId,
                AdditionalProperties = new Dictionary<string, object>()
            };
            
            // Si hay AdditionalProperties, procesarlos para preservar todos los campos
            if (expense.AdditionalProperties != null)
            {
                foreach (var prop in expense.AdditionalProperties)
                {
                    // Si el campo viene con espacios (como "Expected USD"), mapearlo a las propiedades conocidas
                    if (prop.Key.Contains("Expected USD", StringComparison.OrdinalIgnoreCase) && processedExpense.ExpectedUSD == null)
                    {
                        if (decimal.TryParse(prop.Value?.ToString(), out var expectedUsd))
                            processedExpense.ExpectedUSD = expectedUsd;
                    }
                    else if (prop.Key.Contains("Actual USD", StringComparison.OrdinalIgnoreCase) && processedExpense.ActualUSD == null)
                    {
                        if (decimal.TryParse(prop.Value?.ToString(), out var actualUsd))
                            processedExpense.ActualUSD = actualUsd;
                    }
                    else if (prop.Key.Contains("Expected CRC", StringComparison.OrdinalIgnoreCase) && processedExpense.ExpectedCRC == null)
                    {
                        if (decimal.TryParse(prop.Value?.ToString(), out var expectedCrc))
                            processedExpense.ExpectedCRC = expectedCrc;
                    }
                    else if (prop.Key.Contains("Actual CRC", StringComparison.OrdinalIgnoreCase) && processedExpense.ActualCRC == null)
                    {
                        if (decimal.TryParse(prop.Value?.ToString(), out var actualCrc))
                            processedExpense.ActualCRC = actualCrc;
                    }
                    
                    // Preservar todos los campos adicionales para que se guarden en el JSON
                    if (processedExpense.AdditionalProperties != null && prop.Value != null)
                    {
                        processedExpense.AdditionalProperties[prop.Key] = prop.Value;
                    }
                }
            }
            
            processed.Add(processedExpense);
        }
        
        return processed;
    }

    private static decimal ClampDecimal(decimal value)
    {
        if (value > MaxSafeAmount) return MaxSafeAmount;
        if (value < -MaxSafeAmount) return -MaxSafeAmount;
        return value;
    }

    /// <summary>
    /// Acepta "Debit Account", "Payment method", "debit", "credit", etc. y devuelve "debit" o "credit".
    /// </summary>
    private static string? NormalizePaymentMethod(string? paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod)) return null;
        var lower = paymentMethod.Trim().ToLowerInvariant();
        if (lower.Contains("credit")) return "credit";
        if (lower.Contains("debit") || lower.Contains("account") || lower.Contains("payment")) return "debit";
        return paymentMethod.Trim();
    }

    private bool ExpenseExists(Guid id)
    {
        return _context.Expenses.Any(e => e.Id == id);
    }
}
