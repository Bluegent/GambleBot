using System.Threading.Tasks;

namespace BotClient
{
    using System.Threading;

    using BotClient.Discord;

    class Program
    {
        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            const int SLEEP_TIME = 1000;
            DiscordClient discord = new DiscordClient();
            discord.Init();
            while (true)
            {
                Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}
