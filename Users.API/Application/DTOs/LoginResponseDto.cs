namespace Users.API.Application.DTOs;

public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = "Login successful";
}
