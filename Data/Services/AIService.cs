using System.ClientModel;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace iSarv.Data.Services;

public interface IAIService
{
    public Task<(bool IsSuccess, string Reply)> GetChatGPTReplyForNeoTestAsync(string score);

    public Task<(bool IsSuccess, string Reply)> GetAvalAIReplyForTestAsync(string score, string test,
        string language = "Persian", int replyLength = 1000);
}

public class AIService : IAIService
{
    private readonly AISettings _settings;

    public AIService(IOptions<AISettings> options)
    {
        _settings = options.Value;
    }

    public async Task<(bool IsSuccess, string Reply)> GetChatGPTReplyForNeoTestAsync(string score)
    {
        ChatClient client = new ChatClient(model: _settings.OpenAIModel, apiKey: _settings.OpenAIApiKey);

        try
        {
            ChatCompletion completion = await client.CompleteChatAsync(score);
            return (true, completion.Content[0].Text);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }
    
    public async Task<(bool IsSuccess, string Reply)> GetAvalAIReplyForTestAsync(string score, string test, string language = "Persian", int replyLength = 1000)
    {
        ChatClient client = new ChatClient(model: _settings.AvalAIModel,
            new ApiKeyCredential(_settings.AvalAIApiKey), 
            new OpenAIClientOptions()
            {
                Endpoint = new Uri(_settings.AvalAIUri),
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
}

public class AISettings
{
    public string OpenAIApiKey { get; set; } = "";
    public string OpenAIModel { get; set; } = "";
    public string OpenAIUri { get; set; } = "";
    public int MaxTokens { get; set; } = 1000;
    public string AvalAIApiKey { get; set; } = "";
    public string AvalAIModel { get; set; } = "";
    public string AvalAIUri { get; set; } = "";
}