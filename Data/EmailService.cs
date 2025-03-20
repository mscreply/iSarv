using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace iSarv.Data;

public interface IEmailService
{
    public bool SendEmail(string email, string subject, string htmlMessage);
    public Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;

    public EmailService(IOptions<MailSettings> options)
    {
        _mailSettings = options.Value;
    }

    public bool SendEmail(string email, string subject, string htmlMessage)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };
            var client = new SmtpClient();
            client.Connect(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL);
            client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            // Exception Details
            return false;
        }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };
            var client = new SmtpClient();
            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL);
            await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            client.Dispose();
        }
        catch (Exception ex)
        {
            // Exception Details
        }
    }
}

public class MailSettings
{
    public string EmailId { get; set; } = "";
    public string Name { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public bool UseSSL { get; set; }
}