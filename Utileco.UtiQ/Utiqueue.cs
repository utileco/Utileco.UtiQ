using Microsoft.AspNetCore.Http;

namespace Utileco.UtiQ
{
    public class Utiqueue : IUtiQ
    {
        private readonly HttpContext _context;

        public Utiqueue(HttpContext context)
        {
            _context = context;
        }

        public async Task SendCommand<T>(T command)
        {
            await _context.SendCommand(command);
        }

        public async Task SendQuery<T, TResult>(T query)
        {
            await _context.SendQuery<T, TResult>(query);
        }
    }
}
