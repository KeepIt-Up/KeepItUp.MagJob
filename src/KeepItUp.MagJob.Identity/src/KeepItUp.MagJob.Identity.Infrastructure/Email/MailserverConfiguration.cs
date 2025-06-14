namespace KeepItUp.MagJob.Identity.Infrastructure.Email;

/// <summary>
/// Configuration for the mail server
/// </summary>
public class MailserverConfiguration()
{
    /// <summary>
    /// The hostname of the mail server
    /// </summary>
    public string Hostname { get; set; } = "localhost";

    /// <summary>
    /// The port of the mail server
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// Whether to enable SSL/TLS encryption
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// Username for SMTP authentication (if required)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for SMTP authentication (if required)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Default sender email address
    /// </summary>
    public string FromEmail { get; set; } = "noreply@magjob.com";

    /// <summary>
    /// Default sender display name
    /// </summary>
    public string FromName { get; set; } = "MagJob";
}
