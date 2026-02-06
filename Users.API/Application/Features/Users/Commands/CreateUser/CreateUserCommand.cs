using MediatR;
using Users.API.Application.DTOs;

namespace Users.API.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool HaveCreditCards { get; set; }
    public bool HaveLoans { get; set; }
    public decimal? TouringStatus { get; set; }
    public string? PreferredCurrency { get; set; } = "USD";
    // Token se genera automáticamente en el servidor, no se envía
}

