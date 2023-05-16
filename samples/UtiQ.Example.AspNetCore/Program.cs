using Microsoft.Extensions.DependencyInjection;
using Utileco.UtiQ;

namespace UtiQ.Example.AspNetCore
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var ass = BuildUtiQ();
            Console.WriteLine(ass);
        }

        private static IUtiQ BuildUtiQ()
        {
            var services = new ServiceCollection();


            services.AddUtiQ(x => x.RegisterServicesFromAssemblies(typeof(Program).Assembly));

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IUtiQ>();
        }
    }
}