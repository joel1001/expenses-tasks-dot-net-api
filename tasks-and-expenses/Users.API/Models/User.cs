using System.ComponentModel.DataAnnotations;

namespace Users.API.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Password { get; set; }
    
    [MaxLength(255)]
    public string? PasswordHash { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public bool HaveCreditCards { get; set; }
    
    public bool HaveLoans { get; set; }
    
    [MaxLength(255)]
    public string? Token { get; set; }
    
    public decimal? TouringStatus { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; } // Rol del usuario (ej: Admin, User, Manager, etc.)
    
    [MaxLength(3)]
    public string? PreferredCurrency { get; set; } = "USD"; // Moneda preferida del usuario
}

