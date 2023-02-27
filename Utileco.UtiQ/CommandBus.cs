using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Utileco.UtiQ.Command;

namespace Utileco.UtiQ
{
    public static class CommandBus
    {
        public static ICommandHandler<T> GetCommandHandler<T>(this HttpContext context)
            => context.RequestServices.GetRequiredService<ICommandHandler<T>>();

        public static Task SendCommand<T>(this HttpContext context, T command)
            => context.GetCommandHandler<T>().Handle(command, context.RequestAborted);
    }
}
