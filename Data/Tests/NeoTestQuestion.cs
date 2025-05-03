using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iSarv.Data.Tests;

public class NeoTestQuestion
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Statement", Prompt = "Enter the statement")]
    public string Statement { get; set; } = string.Empty;

    public NeoPersonalityInventory PI => Facet switch
    {
        NeoFacets.Fantasy or NeoFacets.Aesthetics or NeoFacets.Feelings or NeoFacets.Actions or NeoFacets.Ideas or NeoFacets.Values => NeoPersonalityInventory.Openness,
        NeoFacets.Competence or NeoFacets.Orderliness or NeoFacets.Dutifulness or NeoFacets.AchievementStriving or NeoFacets.SelfDiscipline or NeoFacets.Deliberation => NeoPersonalityInventory.Conscientiousness,
        NeoFacets.Warmth or NeoFacets.Gregariousness or NeoFacets.Assertiveness or NeoFacets.Activity or NeoFacets.ExcitementSeeking or NeoFacets.PositiveEmotions => NeoPersonalityInventory.Extraversion,
        NeoFacets.Trusting or NeoFacets.Straightforwardness or NeoFacets.Altruism or NeoFacets.Compliance or NeoFacets.Modesty or NeoFacets.Tenderness => NeoPersonalityInventory.Agreeableness,
        NeoFacets.Anxiety or NeoFacets.AngerHostility or NeoFacets.Depression or NeoFacets.SelfConsciousness or NeoFacets.Impulsiveness or NeoFacets.VulnerabilityToStress => NeoPersonalityInventory.Neuroticism,
        _ => throw new ArgumentOutOfRangeException(nameof(Facet), "Unknown facet")
    };

    [Display(Name = "Facet", Prompt = "Select the facet")]
    public NeoFacets Facet { get; set; }

    [Display(Name = "Score Is Reversed", Prompt = "Is the score reversed?")]
    public bool ScoreIsReversed { get; set; } = false;
}

public enum NeoFacets
{
    Fantasy,
    Aesthetics,
    Feelings,
    Actions,
    Ideas,
    Values,
    Competence,
    Orderliness,
    Dutifulness,
    AchievementStriving,
    SelfDiscipline,
    Deliberation,
    Warmth,
    Gregariousness,
    Assertiveness,
    Activity,
    ExcitementSeeking,
    PositiveEmotions,
    Trusting,
    Straightforwardness,
    Altruism,
    Compliance,
    Modesty,
    Tenderness,
    Anxiety,
    [Description("Anger | Hostility")] AngerHostility,
    Depression,
    [Description("Self Consciousness")] SelfConsciousness,
    Impulsiveness,
    [Description("Vulnerability To Stress")] VulnerabilityToStress
}

public enum NeoPersonalityInventory
{
    Openness,
    Conscientiousness,
    Extraversion,
    Agreeableness,
    Neuroticism
}

