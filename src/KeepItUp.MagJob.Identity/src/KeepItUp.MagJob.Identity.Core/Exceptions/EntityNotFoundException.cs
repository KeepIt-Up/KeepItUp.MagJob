namespace KeepItUp.MagJob.Identity.Core.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : Exception
{   
    /// <summary>
    /// Creates a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EntityNotFoundException(string message) : base(message)
    {
    }
}
