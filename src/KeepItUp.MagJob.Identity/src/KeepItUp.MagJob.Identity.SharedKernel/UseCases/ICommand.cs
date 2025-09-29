using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.SharedKernel.UseCases;

/// <summary>
/// Source: https://code-maze.com/cqrs-mediatr-fluentvalidation/
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}
