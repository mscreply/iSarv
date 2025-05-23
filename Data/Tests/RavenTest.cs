using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class RavenTest
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for RavenTest

    [Display(Name = "Test Result", Prompt = "Enter the test result")]
    public string Result { get; set; } = string.Empty; // Result of the test

    [Display(Name = "Is Confirmed", Prompt = "Confirm the test result")]
    public bool IsConfirmed { get; set; } = false; // Indicates if the test is confirmed

    [Display(Name = "Test Package", Prompt = "Select the test package")]
    public TestPackage TestPackage { get; set; } = default!; // Navigation property to TestPackage

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(21); // Start date of the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Deadline", Prompt = "Select the deadline")]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(28); // Deadline for the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Submit Date", Prompt = "Select the submit date")]
    public DateTime SubmitDate { get; set; } = default!; // Date when the test was submitted

    [Display(Name = "Response", Prompt = "Enter the response")]
    public string Response { get; set; } = "";

    // Add properties and methods specific to Raven's Progressive Matrices test
    public bool IsCompleted => !string.IsNullOrEmpty(Result);

    public TimeSpan TimeRemaining => Deadline - DateTime.Now;

    public string Status => IsCompleted ? "Completed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now <= Deadline ? "In Progress" : "Expired";

    private ApplicationDbContext applicationDbContext { get; }

    public Dictionary<string, int> CalculateScores()
    {
        var correctAnswersString = File.ReadAllText(Path.Combine(new HttpContextAccessor().HttpContext?.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .WebRootPath!, "App_Files/data/raven/raven_answers.txt"));
        var correctAnswers = correctAnswersString.Split(',');
        var userResponses = Response.Split(',');

        var categoryScores = new int[5];
        for (var i = 0; i < 60; i++)
        {
            var categoryIndex = i / 12;
            if (userResponses.Length > i && correctAnswers.Length > i)
            {
                if (userResponses[i].Trim() == correctAnswers[i].Trim())
                {
                    categoryScores[categoryIndex]++;
                }
            }
        }

        return new Dictionary<string, int>
        {
            { "A", categoryScores[0] },
            { "B", categoryScores[1] },
            { "C", categoryScores[2] },
            { "D", categoryScores[3] },
            { "E", categoryScores[4] }
        };
    }
}