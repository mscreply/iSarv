using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class HollandTest
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for HollandTest

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
    [Display(Name = "Is Completed", Prompt = "Indicate if the test is completed")]
    public bool IsCompleted { get; set; } = false; // Indicates if the test is completed

    public string Status => IsCompleted ? IsConfirmed ? "Completed" : "Not Confirmed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now <= Deadline ? "In Progress" : "Expired";

    public TimeSpan TimeRemaining => Deadline - DateTime.Now;

    private readonly ApplicationDbContext _applicationDbContext;

    public Dictionary<HollandPersonality, int> CalculateScores()
    {
        // Parse the Response string into an array of integers
        var scores = Response.Split(',').Select(int.Parse).ToArray();

        // Retrieve all questions from the database
        var questions = _applicationDbContext.HollandTestQuestions.ToList();

        // Calculate the score for each Holland personality type
        var personalityScores = new Dictionary<HollandPersonality, int>
        {
            { HollandPersonality.Realistic, 0 },
            { HollandPersonality.Investigative, 0 },
            { HollandPersonality.Artistic, 0 },
            { HollandPersonality.Social, 0 },
            { HollandPersonality.Enterprising, 0 },
            { HollandPersonality.Conventional, 0 }
        };

        for (int i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            var score = scores[i];
            personalityScores[question.HollandPersonality] += score;
        }

        return personalityScores;
    }
}
