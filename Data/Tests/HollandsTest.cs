using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class HollandsTest
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for HollandsTest

    [Display(Name = "Test Result", Prompt = "Enter the test result")]
    public string Result { get; set; } = string.Empty;

    [Display(Name = "Is Confirmed", Prompt = "Confirm the test result")]
    public bool IsConfirmed { get; set; } = false; // Indicates if the test is confirmed

    [Display(Name = "Test Package", Prompt = "Select the test package")]
    public TestPackage TestPackage { get; set; } = default!;

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(14); // Start date of the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Deadline", Prompt = "Select the deadline")]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(21); // Deadline for the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Submit Date", Prompt = "Select the submit date")]
    public DateTime SubmitDate { get; set; } = default!; // Date when the test was submitted

    [Display(Name = "Response", Prompt = "Enter the response")]
    public string Response { get; set; } = "";

    // Add properties and methods specific to Holland's Occupational Themes test
    public bool IsCompleted => !string.IsNullOrEmpty(Result);

    public string Status => IsCompleted ? "Completed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now > Deadline ? "Expired" : "In Progress";

    public TimeSpan TimeRemaining => Deadline - DateTime.Now;

    private ApplicationDbContext applicationDbContext { get; }

    public object CalculateScores()
    {
        throw new NotImplementedException();
    }
}
