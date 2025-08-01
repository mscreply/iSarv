using System.ClientModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace iSarv.Data.Services;

public interface IAIService
{
    public Task<(bool IsSuccess, string Reply)> GetAIReplyForTestAsync(string score, Prompt prompt, ApplicationUser subject, string server = "default", string model = "default");

    public List<string> GetServerList();
    public List<string> GetModelList();
}

public class AIService : IAIService
{
    private readonly ApplicationDbContext _context;

    public AIService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool IsSuccess, string Reply)> GetAIReplyForTestAsync(string score, Prompt prompt, ApplicationUser subject, string server = "default", string model = "default")
    {
        var api_key = "";
        var uri = "";

        var aiSetting = await _context.AISettings.FirstOrDefaultAsync(aiSetting => aiSetting.Server == server);
        if (aiSetting != null)
        {
            api_key = aiSetting.ApiKey;
            uri = aiSetting.Uri;
            model = model == "default" ? aiSetting.DefaultModel : model;
        }

        ChatClient client = new ChatClient(model,
            new ApiKeyCredential(api_key),
            new OpenAIClientOptions()
            {
                Endpoint = new Uri(uri),
            });

        var message = string.IsNullOrEmpty(prompt.Name) ?
        $"You are a psychologist. You are given the scores of a {prompt.Test} test. You need to write a detailed report on the personality of the person who took the test. The report should be based on the scores provided to you. The report should be written in a professional tone and should be easy to understand." :
        prompt.PromptText.Replace("{SubjectInfo}", $"Date of Birth: {subject.DateOfBirth}, Age: {DateTime.Now.Year - subject.DateOfBirth.Year}, Gender: {subject.Gender}, Field of Study: {subject.FieldOfStudy}, Occupation: {subject.Occupation}");

        message = message.Contains("{score}") ? message.Replace("{score}", score) : message + $"\nThe score of the test is: {score}";

        message +=
            $"\nThe report should be in {prompt.Language} and should be at least {prompt.ReplyLength} words long.";

        try
        {
            ChatCompletion completion = await client.CompleteChatAsync(message);
            return (true, completion.Content[0].Text);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    public List<string> GetServerList()
    {
        return _context.AISettings.Select(ai => ai.Server).Distinct().ToList();
    }

    public List<string> GetModelList()
    {
        return string.Join(",", _context.AISettings.Select(ai => ai.ModelList).ToList()).Split(",").Distinct().ToList();
    }
}

public class AISetting
{
    public int Id { get; set; }
    public string Server { get; set; } = "";
    public string Uri { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string DefaultModel { get; set; } = "";
    public string ModelList { get; set; } = "";
    public int MaxTokens { get; set; } = 1000;
}

public class Prompt
{
    [Key]
    [Required]
    [Display(Name = "Name", Prompt = "Enter the prompt name")]
    public string Name { get; set; } = "";

    [Required]
    [Display(Name = "Test", Prompt = "Enter the test name")]
    public Tests Test { get; set; }

    [Required]
    [Display(Name = "Prompt Text", Prompt = "Enter the prompt text")]
    public string PromptText { get; set; } = "";

    [Required]
    [Display(Name = "Language", Prompt = "Enter the language of AI reply")]
    public string Language { get; set; } = "Persian";

    [Required]
    [Display(Name = "Reply Length", Prompt = "Enter the reply length")]
    public int ReplyLength { get; set; } = 1000;
}

public enum Tests
{
    Clifton,
    Holland,
    Raven,
    Neo,
    Package
}