using System.ComponentModel.DataAnnotations;

namespace APBD_LAB12.DTOs;

public class ClientInputDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Telephone { get; set; }
    [Required]
    public string Pesel { get; set; }
    public DateTime? PaymentDate { get; set; }
}