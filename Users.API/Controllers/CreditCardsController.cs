using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.API.Data;
using Users.API.Models;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users/{userId}/credit-cards")]
public class CreditCardsController : ControllerBase
{
    private readonly UsersDbContext _context;
    private readonly ILogger<CreditCardsController> _logger;

    public CreditCardsController(UsersDbContext context, ILogger<CreditCardsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditCard>>> GetCreditCards(int userId, CancellationToken cancellationToken)
    {
        var cards = await _context.CreditCards
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CreditCard>> CreateCreditCard(int userId, [FromBody] CreateCreditCardDto dto, CancellationToken cancellationToken)
    {
        // Validar que el usuario existe
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!userExists)
        {
            return NotFound(new { error = "Usuario no encontrado" });
        }

        // Validar tipo
        if (dto.Type != "Credit" && dto.Type != "Debit")
        {
            return BadRequest(new { error = "El tipo debe ser 'Credit' o 'Debit'" });
        }

        // Validar que tarjetas de crédito tengan días de corte y pago
        if (dto.Type == "Credit" && (dto.CutDay < 1 || dto.CutDay > 31 || dto.PaymentDay < 1 || dto.PaymentDay > 31))
        {
            return BadRequest(new { error = "Las tarjetas de crédito deben tener días de corte y pago válidos (1-31)" });
        }

        var card = new CreditCard
        {
            UserId = userId,
            Name = dto.Name,
            Type = dto.Type,
            CutDay = dto.Type == "Credit" ? dto.CutDay : 0, // Débito no necesita día de corte
            PaymentDay = dto.Type == "Credit" ? dto.PaymentDay : 0, // Débito no necesita día de pago
            CreatedDate = DateTime.UtcNow
        };

        _context.CreditCards.Add(card);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCreditCard), new { userId, id = card.Id }, card);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CreditCard>> GetCreditCard(int userId, int id, CancellationToken cancellationToken)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (card == null)
        {
            return NotFound();
        }

        return Ok(card);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCreditCard(int userId, int id, [FromBody] UpdateCreditCardDto dto, CancellationToken cancellationToken)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (card == null)
        {
            return NotFound();
        }

        if (dto.Type != null && dto.Type != "Credit" && dto.Type != "Debit")
        {
            return BadRequest(new { error = "El tipo debe ser 'Credit' o 'Debit'" });
        }

        card.Name = dto.Name ?? card.Name;
        card.Type = dto.Type ?? card.Type;
        card.CutDay = dto.CutDay ?? card.CutDay;
        card.PaymentDay = dto.PaymentDay ?? card.PaymentDay;
        card.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCreditCard(int userId, int id, CancellationToken cancellationToken)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);

        if (card == null)
        {
            return NotFound();
        }

        _context.CreditCards.Remove(card);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}

public class CreateCreditCardDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Debit"; // Default: Debit
    public int CutDay { get; set; }
    public int PaymentDay { get; set; }
}

public class UpdateCreditCardDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int? CutDay { get; set; }
    public int? PaymentDay { get; set; }
}
