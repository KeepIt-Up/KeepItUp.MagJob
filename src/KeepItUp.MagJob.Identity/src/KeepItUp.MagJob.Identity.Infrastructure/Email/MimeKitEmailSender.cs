using KeepItUp.MagJob.Identity.Core.Interfaces;
using MailKit.Security;
using System.Net;

namespace KeepItUp.MagJob.Identity.Infrastructure.Email;

/// <summary>
/// Implementation of the email sender using MimeKit - recommended approach
/// </summary>
public class MimeKitEmailSender : IEmailSender
{
  private readonly ILogger<MimeKitEmailSender> _logger;
  private readonly MailserverConfiguration _mailserverConfiguration;

  /// <summary>
  /// Initializes a new instance of the MimeKitEmailSender
  /// </summary>
  /// <param name="logger">Logger instance</param>
  /// <param name="mailserverOptions">Mail server configuration options</param>
  public MimeKitEmailSender(ILogger<MimeKitEmailSender> logger,
      IOptions<MailserverConfiguration> mailserverOptions)
  {
    _logger = logger;
    _mailserverConfiguration = mailserverOptions.Value ?? throw new ArgumentNullException(nameof(mailserverOptions));
  }

  /// <inheritdoc />
  public async Task SendEmailAsync(string to, string subject, string body)
  {
    try
    {
      var message = CreateMimeMessage(to, subject, body);
      await SendMimeMessageAsync(message);
      _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}' using {Type}",
          to, subject, nameof(MimeKitEmailSender));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email to {Recipient}: {Error}", to, ex.Message);
      throw;
    }
  }

  /// <summary>
  /// Creates a MIME message with the specified parameters
  /// </summary>
  /// <param name="to">Recipient email address</param>
  /// <param name="from">Sender email address</param>
  /// <param name="subject">Email subject</param>
  /// <param name="body">Email body</param>
  /// <returns>Configured MimeMessage</returns>
  private MimeMessage CreateMimeMessage(string to, string subject, string body)
  {
    var senderEmail = _mailserverConfiguration.FromEmail;
    var senderName = _mailserverConfiguration.FromName;

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(senderName, senderEmail));
    message.To.Add(new MailboxAddress(to, to));
    message.Subject = subject;
    message.Body = new TextPart("html") { Text = body };

    return message;
  }

  /// <summary>
  /// Sends the MIME message using MailKit SMTP client
  /// </summary>
  /// <param name="message">MIME message to send</param>
  private async Task SendMimeMessageAsync(MimeMessage message)
  {
    using var client = new MailKit.Net.Smtp.SmtpClient();

    // Determine security options
    var secureSocketOptions = _mailserverConfiguration.EnableSsl
        ? SecureSocketOptions.StartTls
        : SecureSocketOptions.None;

    await client.ConnectAsync(_mailserverConfiguration.Hostname,
        _mailserverConfiguration.Port, secureSocketOptions);

    // Authenticate if credentials are provided
    if (!string.IsNullOrWhiteSpace(_mailserverConfiguration.Username) &&
        !string.IsNullOrWhiteSpace(_mailserverConfiguration.Password))
    {
      await client.AuthenticateAsync(_mailserverConfiguration.Username,
          _mailserverConfiguration.Password);
    }

    await client.SendAsync(message);
    await client.DisconnectAsync(true);
  }
}
