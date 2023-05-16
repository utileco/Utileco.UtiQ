namespace Utileco.UtiQ
{
    public interface IUtiQ
    {
        Task<TResponse> SendQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

        Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
        Task SendCommand<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : ICommand;
    }
}
