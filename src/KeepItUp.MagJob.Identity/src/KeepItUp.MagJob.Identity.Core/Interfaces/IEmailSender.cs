namespace KeepItUp.MagJob.Identity.Core.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}
