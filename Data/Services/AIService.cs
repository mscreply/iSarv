using System.ClientModel;
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
    private readonly AISettings _settings;

    public AIService(IOptions<AISettings> options)
    {
        _settings = options.Value;
    }

    public async Task<(bool IsSuccess, string Reply)> GetAIReplyForTestAsync(string score, string test, string server = "default", string model = "default", string language = "Persian", int replyLength = 1000)
    {
        var api_key = "";
        var uri = "";
        
        switch (server)
        {
            case "OpenAI":
                api_key = _settings.OpenAIApiKey;
                uri = _settings.OpenAIUri;
                break;
            case "AvalAI" :
                api_key = _settings.AvalAIApiKey;
                uri = _settings.AvalAIUri;
                break;
            case "default" :
                api_key = _settings.ApiKey;
                uri = _settings.Uri;
                break;
        }
        ChatClient client = new ChatClient(model: model == "default" ? _settings.Model : model,
            new ApiKeyCredential(api_key), 
            new OpenAIClientOptions()
            {
                Endpoint = new Uri(uri),
            });
        
        var message = $"You are a psychologist. You are given the scores of a {test} test. You need to write a detailed report on the personality of the person who took the test. The report should be in {language} and should be at least {replyLength} words long. The report should be based on the scores provided to you. The report should be written in a professional tone and should be easy to understand.";
        message += "\n\n"     + score;

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
        return _settings.ServerList.Split(",").ToList();
    }

    public List<string> GetModelList()
    {
        return _settings.ModelList.Split(",").ToList();
    }
}

public class AISettings
{
    public string OpenAIApiKey { get; set; } = "";
    public string OpenAIUri { get; set; } = "";
    public string AvalAIApiKey { get; set; } = "";
    public string AvalAIUri { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string Uri { get; set; } = "";
    public string Model { get; set; } = "";
    public int MaxTokens { get; set; } = 1000;
    public string ServerList { get; set; } = "";
    public string ModelList { get; set; } = "";
}