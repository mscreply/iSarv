using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class CliftonTest
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for CliftonTest

    [Display(Name = "Test Result", Prompt = "Enter the test result")]
    public string Result { get; set; } = string.Empty; // Initialized to avoid nullability issues

    [Display(Name = "Is Confirmed", Prompt = "Confirm the test result")]
    public bool IsConfirmed { get; set; } = false; // Indicates if the test is confirmed
    
    [Display(Name = "Test Package", Prompt = "Select the test package")]
    public TestPackage TestPackage { get; set; } = default!;

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    public DateTime StartDate { get; set; } = DateTime.Now.AddDays(7); // Start date of the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Deadline", Prompt = "Select the deadline")]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(14); // Deadline for the test

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Submit Date", Prompt = "Select the submit date")]
    public DateTime SubmitDate { get; set; } = default!; // Date when the test was submitted

    public bool IsCompleted => !string.IsNullOrEmpty(Result);

    public TimeSpan TimeRemaining => Deadline - DateTime.Now;

    [Display(Name = "Response", Prompt = "Enter the response")]
    public string Response { get; set; } = "";

    public string Status => IsCompleted ? "Completed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now <= Deadline ? "In Progress" : "Expired" ;

    private ApplicationDbContext applicationDbContext { get; }

    public Dictionary<CliftonDomain, Dictionary<CliftonTheme, int>> CalculateScores()
    {
        var scores = new Dictionary<CliftonDomain, Dictionary<CliftonTheme, int>>();

        // Assuming ApplicationDbContext is available and properly configured
        using (applicationDbContext)
        {
            var questions = applicationDbContext.CliftonTestQuestions.ToList();
            var responses = Response.Split("|");

            foreach (var question in questions)
            {
                int.TryParse(responses.ElementAtOrDefault(questions.IndexOf(question)), out int response);

                // Initialize domain and theme scores if not already present
                if (!scores.ContainsKey(question.DomainA))
                    scores[question.DomainA] = new Dictionary<CliftonTheme, int>();

                if (!scores[question.DomainA].ContainsKey(question.ThemeA))
                    scores[question.DomainA][question.ThemeA] = 0;

                if (!scores.ContainsKey(question.DomainB))
                    scores[question.DomainB] = new Dictionary<CliftonTheme, int>();

                if (!scores[question.DomainB].ContainsKey(question.ThemeB))
                    scores[question.DomainB][question.ThemeB] = 0;

                // Assign scores based on the response
                if (response == 2) // Fully Describe Me for A
                {
                    scores[question.DomainA][question.ThemeA] += 2;
                }
                else if (response == 1) // Describe Me for A
                {
                    scores[question.DomainA][question.ThemeA] += 1;
                }
                else if (response == -1) // Describe Me for B
                {
                    scores[question.DomainB][question.ThemeB] += 1;
                }
                else if (response == -2) // Fully Describe Me for B
                {
                    scores[question.DomainB][question.ThemeB] += 2;
                }
                // Neutral (0) does not affect the score
            }

            return scores;
        }
    }
}
