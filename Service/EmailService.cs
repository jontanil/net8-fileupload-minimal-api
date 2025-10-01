using MailKit.Net.Smtp;
using FileUploadApi.Dtos;
using MimeKit;
using Microsoft.Extensions.Options;
using FileUploadApi.Models;


namespace FileUploadApi.Service;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettingsAccessor)
    {
        _emailSettings = emailSettingsAccessor.Value;
    }
    
    public void SendEmail(EmailDto content)
    {
        try
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail);
                MailboxAddress emailTo = new MailboxAddress(content.EmailToName, content.EmailToId);

                emailMessage.From.Add(emailFrom);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = content.EmailSubject;
                
                emailMessage.Body = new BodyBuilder{TextBody = content.EmailBody}.ToMessageBody();

                using (SmtpClient mailClient = new SmtpClient())
                {
                    mailClient.Connect(_emailSettings.Server, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    mailClient.Authenticate(_emailSettings.UserName, _emailSettings.Password);
                    mailClient.Send(emailMessage);
                    mailClient.Disconnect(true);
                }
            }
        }
        catch(Exception Ex)
        {
            Console.WriteLine($"Email not sent, error: {Ex.Message}");
        }
    }
}