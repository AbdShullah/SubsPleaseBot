using System.Threading.Tasks;

namespace DiscordBotTemplateV2
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            return new Startup(args).StartAsync();
        }
    }
}