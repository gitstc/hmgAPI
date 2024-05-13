using System.ComponentModel.DataAnnotations;
namespace hmgAPI.DTOs;

//recieved from client , sent to database
public class RegisterDto
{
    [Required]
    public string CompanyName { get; set; }

    [Required]
    public string CoRegistrationNo { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Mobile { get; set; } // mobile //username 

    [Required]
    public string Address { get; set; }

    [Required]
    public string Password { get; set; }

}