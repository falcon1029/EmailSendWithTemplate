using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

public class EmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
{

        private const string TemplateDirectory = "Templates";

   private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public void SendEmailNormal(string toEmail, string subject, string body)
    {
        using (var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
        {
            smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }
    }

     private string GetEmailBody(string templateName, Dictionary<string, string> placeholders)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, TemplateDirectory, $"{templateName}.html");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Template '{templateName}' not found at '{filePath}'.");
        }

        string templateContent = File.ReadAllText(filePath);

        foreach (var placeholder in placeholders)
        {
            templateContent = templateContent.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return templateContent;
    }

     public void SendEmailWithTemplate(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders)
    {
        var body = GetEmailBody(templateName, placeholders);

        using (var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
        {
            smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine($"Email sent to {toEmail} with subject '{subject}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
