using MediatR;

namespace Users.API.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool HaveCreditCards { get; set; }
    public bool HaveLoans { get; set; }
    public decimal? TouringStatus { get; set; }
    public string? PreferredCurrency { get; set; }
    // Token no se actualiza desde UpdateUserCommand (se maneja por separado si es necesario)
}
