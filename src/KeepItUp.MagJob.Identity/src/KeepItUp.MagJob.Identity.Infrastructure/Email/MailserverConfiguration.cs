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
}
