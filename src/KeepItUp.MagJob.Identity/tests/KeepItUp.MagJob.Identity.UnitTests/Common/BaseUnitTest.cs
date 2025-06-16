using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.Common;

/// <summary>
/// Base class for all unit tests.
/// Provides common setup and utilities for testing.
/// </summary>
public abstract class BaseUnitTest : IDisposable
{
    /// <summary>
    /// AutoFixture instance configured with AutoNSubstitute for automatic mocking.
    /// </summary>
    protected Fixture Fixture { get; }

    /// <summary>
    /// Initializes the base test with AutoFixture configured for unit testing.
    /// </summary>
    protected BaseUnitTest()
    {
        Fixture = new Fixture();

        // Configure AutoFixture to use NSubstitute for mocking
        Fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });

        // Configure AutoFixture to create objects with realistic values
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Configure string generation to be more realistic
        Fixture.Customize<string>(composer => composer.FromFactory(() =>
            Guid.NewGuid().ToString()[..8])); // 8 character random strings
    }

    /// <summary>
    /// Creates a mock object using AutoFixture with NSubstitute.
    /// </summary>
    /// <typeparam name="T">Type to create mock for</typeparam>
    /// <returns>Mock instance</returns>
    protected T CreateMock<T>() where T : class
    {
        return Fixture.Create<T>();
    }

    /// <summary>
    /// Creates an instance with automatic property population.
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <returns>Instance with populated properties</returns>
    protected T Create<T>()
    {
        return Fixture.Create<T>();
    }

    /// <summary>
    /// Creates multiple instances of specified type.
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <param name="count">Number of instances to create</param>
    /// <returns>Collection of instances</returns>
    protected IEnumerable<T> CreateMany<T>(int count = 3)
    {
        return Fixture.CreateMany<T>(count);
    }

    /// <summary>
    /// Generates a random GUID for testing.
    /// </summary>
    protected Guid GenerateId() => Guid.NewGuid();

    /// <summary>
    /// Generates a random email address for testing.
    /// </summary>
    protected string GenerateEmail() => $"test{Guid.NewGuid().ToString()[..8]}@example.com";

    /// <summary>
    /// Generates a random string of specified length.
    /// </summary>
    /// <param name="length">Length of the string</param>
    /// <returns>Random string</returns>
    protected string GenerateString(int length = 10)
    {
        return Guid.NewGuid().ToString("N")[..Math.Min(length, 32)];
    }

    /// <summary>
    /// Generates a future date for testing.
    /// </summary>
    /// <param name="daysFromNow">Number of days from now</param>
    /// <returns>Future date</returns>
    protected DateTime GenerateFutureDate(int daysFromNow = 7)
    {
        return DateTime.UtcNow.AddDays(daysFromNow);
    }

    /// <summary>
    /// Generates a past date for testing.
    /// </summary>
    /// <param name="daysAgo">Number of days ago</param>
    /// <returns>Past date</returns>
    protected DateTime GeneratePastDate(int daysAgo = 7)
    {
        return DateTime.UtcNow.AddDays(-daysAgo);
    }

    /// <summary>
    /// Cleanup resources.
    /// </summary>
    public virtual void Dispose()
    {
        // Override in derived classes if cleanup is needed
        GC.SuppressFinalize(this);
    }
}