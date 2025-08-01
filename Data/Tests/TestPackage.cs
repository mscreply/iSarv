using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class TestPackage
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Package Name", Prompt = "Enter the package name")]
    public string Name { get; set; } = "Talent And Strength Package";

    [Display(Name = "Final Result", Prompt = "Enter the final result")]
    public string FinalResult { get; set; } = string.Empty;

    [Display(Name = "User", Prompt = "Select the user")]
    public ApplicationUser? User { get; set; } = null!;

    [Display(Name = "User ID", Prompt = "Enter the user ID")]
    public string UserId { get; set; } = string.Empty;

    // Tests --------------------------------------------------------------------------------

    [Display(Name = "Neo Test", Prompt = "Select the Neo test")]
    public NeoTest? NeoTest { get; set; } = new();

    [Display(Name = "Clifton Test", Prompt = "Select the Clifton test")]
    public CliftonTest CliftonTest { get; set; } = new();

    [Display(Name = "Holland Test", Prompt = "Select the Holland test")]
    public HollandTest? HollandTest { get; set; } = new();

    [Display(Name = "Raven Test", Prompt = "Select the Raven test")]
    public RavenTest RavenTest { get; set; } = new();

    // Dates ----------------------------------------------------------------------------------

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date", Prompt = "Select the start date")]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Deadline", Prompt = "Select the deadline")]
    public DateTime Deadline { get; set; } = DateTime.Now.AddDays(28);

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Submit Date", Prompt = "Select the submit date")]
    public DateTime SubmitDate { get; set; } = default!;

    public bool IsCompleted => !string.IsNullOrEmpty(FinalResult) && FinalResult != "Wait for AI";

    public string Status => IsCompleted ? "Completed" :
        DateTime.Now < StartDate ? "Not Started" :
        DateTime.Now <= Deadline ? "In Progress" : "Expired";

}
