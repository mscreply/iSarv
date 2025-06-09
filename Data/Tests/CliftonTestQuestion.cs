using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class CliftonTestQuestion
{
    [Key] // Added primary key annotation
    public int Id { get; set; } // Primary key for CliftonTestQuestion

    [Display(Name = "Statement A", Prompt = "Enter Statement A")]
    public string StatementA { get; set; } = string.Empty; // Initialized to avoid nullability issues

    public CliftonDomain DomainA => GetDomainFromTheme(ThemeA); // Domain for StatementA

    [Display(Name = "Theme A", Prompt = "Select Theme A")]
    public CliftonTheme ThemeA { get; set; } // Theme for StatementA

    [Display(Name = "Statement B", Prompt = "Enter Statement B")]
    public string StatementB { get; set; } = string.Empty; // Initialized to avoid nullability issues

    [Display(Name = "Theme B", Prompt = "Select Theme B")]
    public CliftonTheme ThemeB { get; set; } // Theme for StatementB

    public CliftonDomain DomainB => GetDomainFromTheme(ThemeB); // Domain for StatementB

    public static CliftonDomain GetDomainFromTheme(CliftonTheme theme)
    {
        return theme switch
        {
            CliftonTheme.Achiever or
                CliftonTheme.Arranger or
                CliftonTheme.Belief or
                CliftonTheme.Consistency or
                CliftonTheme.Deliberative or
                CliftonTheme.Discipline or
                CliftonTheme.Focus or
                CliftonTheme.Responsibility or
                CliftonTheme.Restorative => CliftonDomain.Executing,
            CliftonTheme.Activator or
                CliftonTheme.Command or
                CliftonTheme.Communication or
                CliftonTheme.Competition or
                CliftonTheme.Maximizer or
                CliftonTheme.SelfAssurance or
                CliftonTheme.Significance or
                CliftonTheme.Woo => CliftonDomain.Influencing,
            CliftonTheme.Adaptability or
                CliftonTheme.Connectedness or
                CliftonTheme.Developer or
                CliftonTheme.Empathy or
                CliftonTheme.Harmony or
                CliftonTheme.Includer or
                CliftonTheme.Individualization or
                CliftonTheme.Positivity or
                CliftonTheme.Relator => CliftonDomain.RelationshipBuilding,
            CliftonTheme.Analytical or
                CliftonTheme.Context or
                CliftonTheme.Futuristic or
                CliftonTheme.Ideation or
                CliftonTheme.Input or
                CliftonTheme.Intellection or
                CliftonTheme.Learner or
                CliftonTheme.Strategic => CliftonDomain.StrategicThinking,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), "Unknown theme")
        };
    }
}

public enum CliftonTheme
{
    Achiever,
    Arranger,
    Belief,
    Consistency,
    Deliberative,
    Discipline,
    Focus,
    Responsibility,
    Restorative,
    Activator,
    Command,
    Communication,
    Competition,
    Maximizer,
    SelfAssurance,
    Significance,
    Woo,
    Adaptability,
    Connectedness,
    Developer,
    Empathy,
    Harmony,
    Includer,
    Individualization,
    Positivity,
    Relator,
    Analytical,
    Context,
    Futuristic,
    Ideation,
    Input,
    Intellection,
    Learner,
    Strategic
}

public enum CliftonDomain
{
    Executing,
    Influencing,
    [Description("Relationship Building")] RelationshipBuilding,
    [Description("Strategic Thinking")] StrategicThinking
}