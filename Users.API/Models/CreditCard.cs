using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.API.Models;

public class CreditCard
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = "Debit"; // "Credit" o "Debit" - Default: Debit
    
    [Range(0, 31)]
    public int CutDay { get; set; } // Día del mes en que se corta (1-31) - Solo para crédito, 0 para débito
    
    [Range(0, 31)]
    public int PaymentDay { get; set; } // Día del mes en que se paga (1-31) - Solo para crédito, 0 para débito
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    // Navigation property
    [ForeignKey("UserId")]
    public User? User { get; set; }
}
