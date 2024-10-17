public interface IEmailService
{
    void SendEmailWithTemplate(string toEmail, string subject, string templateName, Dictionary<string, string> placeholders);

    void SendEmailNormal(string toEmail, string subject, string body);
}
