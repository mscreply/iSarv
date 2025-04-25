using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace iSarv.Data;

public enum Gender
{
    Male,
    Female
}

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Full Name", Prompt = "Enter Full Name")]
    public string FullName { get; set; } = "";

    [Display(Name = "Date of Birth", Prompt = "Enter Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Now;

    [Display(Name = "Gender", Prompt = "Select Gender")]
    public Gender Gender { get; set; }

    [Display(Name = "Address", Prompt = "Enter Address")]
    public string Address { get; set; } = "";

    [Display(Name = "Biography", Prompt = "Enter Biography")]
    [DataType(DataType.MultilineText)]
    public string Bio { get; set; } = "";

    [Display(Name = "Is Active", Prompt = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Created Date", Prompt = "Enter Created Date")]
    [DataType(DataType.Date)]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Display(Name = "Last Login Date", Prompt = "Enter Last Login Date")]
    [DataType(DataType.DateTime)]
    public DateTime LastLoginDate { get; set; } = DateTime.Now;

    [Display(Name = "Occupation", Prompt = "Enter Occupation")]
    public string Occupation { get; set; } = "";

    [Display(Name = "Field of Study", Prompt = "Enter Field of Study")]
    public string FieldOfStudy { get; set; } = "";

    [Display(Name = "University", Prompt = "Enter University")]
    public string University { get; set; } = "";

    public List<TestPackage> TestPackages { get; set; }

    public ApplicationUser()
    {
        TestPackages = new List<TestPackage>();
    }

    public bool IsPsychologist { get; set; }
}
