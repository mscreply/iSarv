using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class NeoTest
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for NeoTest

    [Display(Name = "Test Result", Prompt = "Enter the test result")]
    public string Result { get; set; } = string.Empty; // Result of the test

    [Display(Name = "Is Confirmed", Prompt = "Confirm the test result")]
    public bool IsConfirmed { get; set; } = false; // Indicates if the test is confirmed

    [Display(Name = "Test Package", Prompt = "Select the test package")]
    public TestPackage TestPackage { get; set; } = default!; // Navigation property to TestPackage

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    public DateTime StartDate { get; set; } = DateTime.Now; // Start date of the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Deadline", Prompt = "Select the deadline")]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7); // Deadline for the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Submit Date", Prompt = "Select the submit date")]
    public DateTime SubmitDate { get; set; } = default!; // Date when the test was submitted

    // Add properties and methods specific to NEO Personality Inventory test
    [Display(Name = "Is Completed", Prompt = "Indicate if the test is completed")]
    public bool IsCompleted { get; set; } = false; // Indicates if the test is completed

    public TimeSpan TimeRemaining => Deadline - DateTime.Now;

    [Display(Name = "Response", Prompt = "Enter the response")]
    public string Response { get; set; } = "";

    public string Status => IsCompleted ? IsConfirmed ? "Completed" : "Not Confirmed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now <= Deadline ? "In Progress" : "Expired";

    private ApplicationDbContext applicationDbContext { get; }

    public Dictionary<NeoPersonalityInventory, Dictionary<NeoFacets, int>> CalculateScores()
    {
        var scores = new Dictionary<NeoPersonalityInventory, Dictionary<NeoFacets, int>>();

        // Split the response string into individual answers
        var responses = Response.Split(',');

        // Retrieve all questions from the database
        var questions = applicationDbContext.NeoTestQuestions.ToList();

        foreach (var question in questions)
        {
            // Ensure the PI key exists
            if (!scores.ContainsKey(question.PI))
            {
                scores[question.PI] = new Dictionary<NeoFacets, int>();
            }

            // Ensure the Facet key exists within the PI
            if (!scores[question.PI].ContainsKey(question.Facet))
            {
                scores[question.PI][question.Facet] = 0;
            }

            // Parse the response for the current question
            int responseValue = int.TryParse(responses[question.Id - 1], out var value) ? value : 0;

            // Add the score to the corresponding facet within the PI
            scores[question.PI][question.Facet] += responseValue;
        }

        return scores;
    }
}
