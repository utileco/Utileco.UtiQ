using Microsoft.Extensions.DependencyInjection;

namespace Utileco.UtiQ.Wrapper
{
    public abstract class RequestHandlerBase
    {
        public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public abstract class QueryHandler<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(IQuery<TResponse> query, IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public abstract class CommandHandler<TResponse> : RequestHandlerBase
    {
        public abstract Task<TResponse> Handle(ICommand<TResponse> command, IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public abstract class CommandHandler : RequestHandlerBase
    {
        public abstract Task Handle(ICommand command, IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    public class QueryHandlerImp<TRequest, TResponse> : QueryHandler<TResponse>
        where TRequest : IQuery<TResponse>
    {
        public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
            await Handle((IQuery<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);

        public override Task<TResponse> Handle(IQuery<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
        {
            Task<TResponse> Handler() => serviceProvider.GetRequiredService<IQueryHandler<TRequest, TResponse>>()
                .Handle((TRequest)request, cancellationToken);

            return serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                    (next, pipeline) => () => pipeline.Handle((TRequest)request, next, cancellationToken))();
        }
    }

    public class CommandHandlerImp<TRequest, TResponse> : CommandHandler<TResponse>
        where TRequest : ICommand<TResponse>
    {
        public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
            await Handle((ICommand<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);

        public override Task<TResponse> Handle(ICommand<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
        {
            Task<TResponse> Handler() => serviceProvider.GetRequiredService<ICommandHandler<TRequest, TResponse>>()
                .Handle((TRequest)request, cancellationToken);

            return serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                    (next, pipeline) => () => pipeline.Handle((TRequest)request, next, cancellationToken))();
        }
    }

    public class CommandHandlerImp<TRequest> : CommandHandler
        where TRequest : ICommand
    {
        public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
            await Handle(request, serviceProvider, cancellationToken).ConfigureAwait(false);

        public override Task Handle(ICommand request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
        {
            Task Handler() => serviceProvider.GetRequiredService<ICommandHandler<TRequest>>()
                .Handle((TRequest)request, cancellationToken);

            return serviceProvider
                .GetServices<IPipelineBehavior<TRequest>>()
                .Reverse()
                .Aggregate((RequestHandlerDelegate)Handler,
                    (next, pipeline) => () => pipeline.Handle((TRequest)request, next, cancellationToken))();
        }
    }
}
