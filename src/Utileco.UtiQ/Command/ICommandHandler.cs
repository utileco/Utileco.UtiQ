using Utileco.UtiQ.Contracts;

namespace Utileco.UtiQ.Command
{
    public interface ICommandHandler<in TRequest>
        where TRequest : ICommand
    {
        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(TRequest command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        /// <summary>
        /// Handles command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken);
    }
}
