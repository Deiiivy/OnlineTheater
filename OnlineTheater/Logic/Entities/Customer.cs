namespace Logic.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Customer : Entity
{
    [Required]
    [MaxLength(100, ErrorMessage = "Name is too long")]
    public required string Name { get; set; }

    [Required]
    [RegularExpression(@"^(.+)@(.+)$", ErrorMessage = "Email is invalid")]
    public string? Email { get; set; }

    public CustomerStatus Status { get; set; }

    public DateTime? StatusExpirationDate { get; set; }

    public decimal MoneySpent { get; set; }
}