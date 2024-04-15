using System.ComponentModel.DataAnnotations;
namespace hmgAPI.DTOs;

//recieved from client , sent to database
public class RegisterDto
{
    [Required]
    public string CompanyName { get; set; } //companyName 

    [Required]
    public string CoRegistrationNo { get; set; } //companyRegistrationNo

    [Required]
    public string Email { get; set; } //email

    [Required]
    public string Mobile { get; set; } // mobile //username 

    [Required]
    public string Address { get; set; } //address 

    [Required]
    public string Password { get; set; } //password

}