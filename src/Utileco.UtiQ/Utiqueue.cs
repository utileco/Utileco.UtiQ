using System.Collections.Concurrent;
using Utileco.UtiQ.Contracts;
using Utileco.UtiQ.Wrapper;

namespace Utileco.UtiQ
{
    public class Utiqueue : IUtiQ
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlers = new();

        public Utiqueue(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> SendCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = (CommandHandler<TResponse>)_requestHandlers.GetOrAdd(command.GetType(), static commandType =>
            {
                var wrapType = typeof(CommandHandlerImp<,>).MakeGenericType(commandType, typeof(TResponse));
                var wrap = Activator.CreateInstance(wrapType) ?? throw new InvalidOperationException($"Could not create wrapper type for {commandType}");
                return (RequestHandlerBase)wrap;
            });

            return handler.Handle(command, _serviceProvider, cancellationToken);
        }

        public Task SendCommand<TRequest>(TRequest command, CancellationToken cancellationToken = default) where TRequest : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = (CommandHandler)_requestHandlers.GetOrAdd(command.GetType(), static commandType =>
            {
                var wrapType = typeof(CommandHandlerImp<>).MakeGenericType(commandType);
                var wrap = Activator.CreateInstance(wrapType) ?? throw new InvalidOperationException($"Could not create wrapper type for {commandType}");
                return (RequestHandlerBase)wrap;
            });

            return handler.Handle(command, _serviceProvider, cancellationToken);
        }

        public Task<TResponse> SendQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = (QueryHandler<TResponse>)_requestHandlers.GetOrAdd(query.GetType(), static queryType =>
            {
                var wrapType = typeof(QueryHandlerImp<,>).MakeGenericType(queryType, typeof(TResponse));
                var wrap = Activator.CreateInstance(wrapType) ?? throw new InvalidOperationException($"Could not create wrapper type for {queryType}");
                return (RequestHandlerBase)wrap;
            });

            return handler.Handle(query, _serviceProvider, cancellationToken);
        }
    }
}
