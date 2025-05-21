using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class HollandTestQuestion
{
    public int Id { get; set; }

    [Display(Name = "Statement", Prompt = "Enter the statement")]
    public string Statement { get; set; } = "";

    [Display(Name = "Holland Personality Type", Prompt = "Holland Personality Type")]
    public HollandPersonality HollandPersonality { get; set; }

    [Display(Name = "Question Type", Prompt = "Question Type")]
    public HollandQuestionType Type { get; set; }
}

public enum HollandQuestionType
{
    LikeTo,
    CanDo,
    Skill,
    Ability
}

public enum HollandPersonality
{
    Realistic,
    Investigative,
    Artistic,
    Social,
    Enterprising,
    Conventional
}