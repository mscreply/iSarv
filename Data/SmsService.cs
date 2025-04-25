using Microsoft.Extensions.Options;
using RestSharp;

namespace iSarv.Data;

public interface ISmsService
{
    public string SendSmsByPattern(string templateKey, long destination, string p1="", string p2="", string p3="");
    public string SendSms(string text, string recipients);
}

public class SmsService : ISmsService
{
    private readonly SmsSettings _smsSetting;

    public SmsService(IOptions<SmsSettings> options)
    {
        _smsSetting = options.Value;
    }

    public string SendSmsByPattern(string templateKey, long destination, string p1="", string p2="", string p3="")
    {
        var client = new RestClient(_smsSetting.SendTokenSingleUri);
        var request = new RestRequest(_smsSetting.SendTokenSingleUri, Method.Post);
        request.AddHeader("Content-Type", "application/json");
        string body = $@"{{""ApiKey"": ""{_smsSetting.ApiKey}"",""TemplateKey"": ""{templateKey}"",""Destination"": {destination},""P1"": ""{p1}"",""P2"": ""{p2}"",""P3"": ""{p3}""}}";
        request.AddParameter("application/json", body,  ParameterType.RequestBody);
        var response = client.Execute(request);
        return response.Content?? "Unknown";
    }

    public string SendSms(string text, string recipients)
    {
        var client = new RestClient($"{_smsSetting.SendSmsUri}?ApiKey={_smsSetting.ApiKey}&Text={text}&Sender={_smsSetting.Sender}&Recipients={recipients}");
        var request = new RestRequest();
        var response = client.Execute(request);
        return response.Content?? "Unknown";
    }
}

public class SmsSettings
{
    public string ApiKey { get; set; } = "";
    public string Sender { get; set; } = "";
    public string SendTokenSingleUri { get; set; } = "";
    public string SendSmsUri { get; set; } = "";
}