using Microsoft.Extensions.DependencyInjection;
using Utileco.UtiQ.Command;
using Utileco.UtiQ.Query;

namespace Utileco.UtiQ
{
    public static class UtiQConfiguration
    {
        public static void AddCommandHandlers<TCommand, TCommandHandler>(this IServiceCollection services)
            where TCommandHandler : class, ICommandHandler<TCommand>
        {
            services.AddTransient<TCommandHandler>()
                .AddTransient<ICommandHandler<TCommand>>(x => x.GetRequiredService<TCommandHandler>());
        }

        public static void AddQueryHandler<TQuery, TQueryResult, TQueryHandler>(this IServiceCollection services)
            where TQueryHandler : class, IQueryHandler<TQuery, TQueryResult>
        {
            services.AddTransient<TQueryHandler>()
                .AddTransient<IQueryHandler<TQuery, TQueryResult>>(x => x.GetRequiredService<TQueryHandler>());
        }
    }
}
