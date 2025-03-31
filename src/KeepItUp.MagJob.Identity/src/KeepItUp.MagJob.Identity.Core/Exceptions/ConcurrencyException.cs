namespace KeepItUp.MagJob.Identity.Core.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message)
    {
    }
}
