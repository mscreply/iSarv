using System.ClientModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace iSarv.Data.Services;

public interface IAIService
{
    public Task<(bool IsSuccess, string Reply)> GetAIReplyForTestAsync(string score, string test, string server = "default", string model = "default",
        string language = "Persian", int replyLength = 1000);

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

    public async Task<(bool IsSuccess, string Reply)> GetAIReplyForTestAsync(string score, string test, string server = "default", string model = "default", string language = "Persian", int replyLength = 1000)
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

        var message = $"You are a psychologist. You are given the scores of a {test} test. You need to write a detailed report on the personality of the person who took the test. The report should be in {language} and should be at least {replyLength} words long. The report should be based on the scores provided to you. The report should be written in a professional tone and should be easy to understand.";
        message += "\n\n" + score;

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