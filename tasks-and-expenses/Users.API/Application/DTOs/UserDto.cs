namespace Users.API.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool HaveCreditCards { get; set; }
    public bool HaveLoans { get; set; }
    public string? Token { get; set; }
    public decimal? TouringStatus { get; set; }
    public string? Role { get; set; }
    public string? PreferredCurrency { get; set; }
    // Password y PasswordHash NO se incluyen por seguridad
}
