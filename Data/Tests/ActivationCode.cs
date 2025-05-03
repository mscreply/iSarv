using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class ActivationCode
{
    [Key]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Activation Code", Prompt = "Enter the activation code")]
    public string Code { get; set; } = string.Empty;

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Email Address", Prompt = "Enter your email address")]
    public string Email { get; set; } = string.Empty; // Default to empty string

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "Phone Number", Prompt = "Enter your phone number")]
    public string PhoneNumber { get; set; } = string.Empty; // Default to empty string

    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [Display(Name = "National ID", Prompt = "Enter your national ID")]
    public string NationalId { get; set; } = string.Empty; // Default to empty string
}
