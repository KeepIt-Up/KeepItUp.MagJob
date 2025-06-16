using NSubstitute;
using Microsoft.Extensions.Logging;
using MediatR;

namespace KeepItUp.MagJob.Identity.UnitTests.Common.Factories;

/// <summary>
/// Factory for creating mock objects for external services and dependencies.
/// Provides pre-configured mocks with common behaviors.
/// </summary>
public static class MockFactory
{
    /// <summary>
    /// Creates a mock ILogger for the specified type.
    /// </summary>
    /// <typeparam name="T">Type the logger is for</typeparam>
    /// <returns>Mock ILogger instance</returns>
    public static ILogger<T> CreateLogger<T>()
    {
        return Substitute.For<ILogger<T>>();
    }

    /// <summary>
    /// Creates a mock IMediator with default behaviors.
    /// </summary>
    /// <returns>Mock IMediator instance</returns>
    public static IMediator CreateMediator()
    {
        var mediator = Substitute.For<IMediator>();

        // Setup default successful responses for common operations
        mediator.Send(Arg.Any<IRequest>()).Returns(Task.CompletedTask);
        mediator.Send(Arg.Any<IRequest<Unit>>()).Returns(Unit.Value);

        return mediator;
    }

    /// <summary>
    /// Creates a mock IMediator that returns specified results for commands/queries.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="response">Response to return</param>
    /// <returns>Mock IMediator instance</returns>
    public static IMediator CreateMediator<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<TResponse>
    {
        var mediator = CreateMediator();
        mediator.Send(Arg.Any<TRequest>()).Returns(response);
        return mediator;
    }

    /// <summary>
    /// Creates a mock IMediator that throws specified exception.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <param name="exception">Exception to throw</param>
    /// <returns>Mock IMediator instance</returns>
    public static IMediator CreateFailingMediator<TRequest>(Exception exception)
        where TRequest : IRequest
    {
        var mediator = CreateMediator();
        mediator.Send(Arg.Any<TRequest>()).Returns(Task.FromException(exception));
        return mediator;
    }

    /// <summary>
    /// Creates a mock for any repository interface.
    /// </summary>
    /// <typeparam name="T">Repository interface type</typeparam>
    /// <returns>Mock repository instance</returns>
    public static T CreateRepository<T>() where T : class
    {
        return Substitute.For<T>();
    }

    /// <summary>
    /// Creates a mock HttpClient for external API calls.
    /// </summary>
    /// <returns>Mock HttpClient instance</returns>
    public static HttpClient CreateHttpClient()
    {
        // For HttpClient mocking, we typically use HttpMessageHandler
        var handler = Substitute.For<HttpMessageHandler>();
        return new HttpClient(handler);
    }

    /// <summary>
    /// Creates a mock for external service interfaces.
    /// </summary>
    /// <typeparam name="T">Service interface type</typeparam>
    /// <returns>Mock service instance</returns>
    public static T CreateExternalService<T>() where T : class
    {
        return Substitute.For<T>();
    }

    /// <summary>
    /// Creates a mock service that returns successful results.
    /// </summary>
    /// <typeparam name="TService">Service interface type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="result">Result to return</param>
    /// <returns>Mock service instance</returns>
    public static TService CreateSuccessfulService<TService, TResult>(TResult result)
        where TService : class
    {
        var service = Substitute.For<TService>();
        // This would need to be configured per specific service interface
        return service;
    }

    /// <summary>
    /// Creates a mock service that throws exceptions.
    /// </summary>
    /// <typeparam name="TService">Service interface type</typeparam>
    /// <param name="exception">Exception to throw</param>
    /// <returns>Mock service instance</returns>
    public static TService CreateFailingService<TService>(Exception exception)
        where TService : class
    {
        var service = Substitute.For<TService>();
        // This would need to be configured per specific service interface
        return service;
    }
}