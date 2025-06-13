using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.Infrastructure.Email;

/// <summary>
/// Implementation of the email sender
/// </summary>
public class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    private readonly ILogger<FakeEmailSender> _logger = logger;

    /// <inheritdoc />
    public Task SendEmailAsync(string to, string from, string subject, string body)
    {
        _logger.LogInformation("Not actually sending an email to {to} from {from} with subject {subject}", to, from, subject);
        return Task.CompletedTask;
    }
}
