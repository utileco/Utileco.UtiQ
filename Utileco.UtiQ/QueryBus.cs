using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Utileco.UtiQ.Query;

namespace Utileco.UtiQ
{
    public static class QueryBus
    {
        public static IQueryHandler<T, TResult> GetQueryHandler<T, TResult>(this HttpContext context)
        => context.RequestServices.GetRequiredService<IQueryHandler<T, TResult>>();

        public static Task<TResult> SendQuery<T, TResult>(this HttpContext context, T query)
            => context.GetQueryHandler<T, TResult>().Handle(query, context.RequestAborted);
    }
}
