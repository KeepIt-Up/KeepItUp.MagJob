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
    /// Generates a random GUID for testing.
    /// </summary>
    protected Guid GenerateId() => Guid.NewGuid();

    /// <summary>
    /// Generates a random email address for testing.
    /// </summary>
    protected string GenerateEmail() => $"test{Guid.NewGuid().ToString()[..8]}@example.com";

    /// <summary>
    /// Cleanup resources.
    /// </summary>
    public virtual void Dispose()
    {
        // Override in derived classes if cleanup is needed
        GC.SuppressFinalize(this);
    }
}