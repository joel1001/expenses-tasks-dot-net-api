using MediatR;
using Users.API.Application.DTOs;

namespace Users.API.Application.Features.Users.Commands.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
