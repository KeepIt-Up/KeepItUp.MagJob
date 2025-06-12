namespace KeepItUp.MagJob.Identity.Core.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs.
/// </summary>
public class ConcurrencyException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="ConcurrencyException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConcurrencyException(string message) : base(message)
    {
    }
}
