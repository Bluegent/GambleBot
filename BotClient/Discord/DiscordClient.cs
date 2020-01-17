using System;
using System.Threading.Tasks;

namespace BotClient.Discord
{
    using global::Discord;
    using global::Discord.WebSocket;

    using Newtonsoft.Json;

    public class DiscordClient
    {

        public DiscordSocketClient Client { get; private set; }
        public Action ReadyAction { get; set; }

        public bool IsReady { get; private set; }

        private DiscordConfig _discordConfig;


        public SocketGuild Server { get; private set; }
        public SocketTextChannel Channel { get; private set; }


        public async void Init()
        {
            _discordConfig = JsonConvert.DeserializeObject<DiscordConfig>(Utils.FileHandler.Read("_discordConfig.json"));
            Client = new DiscordSocketClient();

            Client.Log += DiscordLog;
            Client.MessageReceived += MessageReceived;
            Client.Ready += ClientReady;
            IsReady = false;

            await Client.LoginAsync(TokenType.Bot, _discordConfig.Token);
            await Client.StartAsync();
        }

        SocketGuild GetServer()
        {
            foreach (SocketGuild s in Client.Guilds)
            {
                if (s.Name.Equals(_discordConfig.Server))
                    return s;
            }
            return null;
        }
        SocketTextChannel GetChannel()
        {
            if (Server != null)
            {
                foreach (SocketTextChannel c in Server.TextChannels)
                    if (c.Name.Equals(_discordConfig.Channel))
                        return c;
            }
            return null;
        }

        private Task ClientReady()
        {
            Server = GetServer();
            if (Server != null)
            {
                Channel = GetChannel();
            }
            IsReady = true;
            //WriteMessage($"The ***Soul Master*** awakens...");
            ReadyAction?.Invoke();

            return Task.CompletedTask;
        }

        private async Task<Task> MessageReceived(SocketMessage message)
        {
            if (message.Content.Equals("ping") && !message.Channel.Equals(Channel))
            {
                await message.Author.SendMessageAsync("pong");
            }
            return Task.CompletedTask;
        }


        private Task DiscordLog(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }

        public void WriteMessage(string msg)
        {
            if (IsReady)
                if (!string.IsNullOrEmpty(msg))
                    Channel.SendMessageAsync(msg);
        }
    }
}
