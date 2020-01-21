using System;
using System.Text;
using System.Threading.Tasks;
using BotClient.Game;

namespace BotClient.Discord
{
    using System.Threading;

    using global::Discord;
    using global::Discord.WebSocket;

    using Newtonsoft.Json;

    public class DiscordClient : ILogger
    {

        public DiscordSocketClient Client { get; private set; }
        public Action ReadyAction { get; set; }

        private Thread _salary;

        public bool IsReady { get; private set; }

        private DiscordConfig _discordConfig;


        public SocketGuild Server { get; private set; }
        public SocketTextChannel Channel { get; private set; }
        private Random _gen;


        private GameController _game;


        public async void Init()
        {
            _discordConfig = JsonConvert.DeserializeObject<DiscordConfig>(Utils.FileHandler.Read("_discordConfig.json"));
            Client = new DiscordSocketClient();

            Client.Log += DiscordLog;
            Client.MessageReceived += MessageReceived;
            Client.Ready += ClientReady;
            IsReady = false;
            _gen = new Random();
            _buffer = new StringBuilder();
            _game = new GameController(this, _gen);

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
            WriteMessage($"Type `{Const.Comm.HELP}` for some info. Let the games begin...");


            _salary = new Thread(
                () =>
                    {
                        bool allocated = false;
                        while (true)
                        {
                            if (DateTime.Now.TimeOfDay.Minutes == 0 || DateTime.Now.TimeOfDay.Minutes == 30)
                            {
                                if (!allocated)
                                {
                                    _game.AllocateSalary();
                                    allocated = true;
                                }
                            }
                            else
                            {
                                if (allocated)
                                {
                                    allocated = false;
                                }
                            }

                            Thread.Sleep(15000);
                        }
                    }
                );

            _salary.Start();
            Flush();

            ReadyAction?.Invoke();

            return Task.CompletedTask;
        }

        private async Task<Task> MessageReceived(SocketMessage message)
        {
            if (message.Channel.Name == "@Bluegent#3495" && message.Author.Id == 446704905944694784)
            {
                string[] bits = message.Content.Trim().Split(' ');
                if (bits[0] == "backup")
                {
                    _game.Backup(message.Channel);
                }
                else if (bits[0] == "restore")
                {
                    string compressed = message.Content.Substring(8);
                    if (compressed.Length == 0)
                    {
                        await message.Channel.SendMessageAsync("Incorrect restore string.");
                    }
                    else if (!_game.Restore(compressed))
                    {
                        await message.Channel.SendMessageAsync(
                            "Something went wrong when decompressing or deserializing.");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Backup restored succesfully and saved locally.");
                    }
                }
            }

            else if (message.Channel.Equals(Channel))
            {
                _game.HandleMessage(message);
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
            _buffer.Append(msg);
            _buffer.Append("\n");
        }

        public async void Flush()
        {
            string res = _buffer.ToString();
            _buffer.Clear();
            if (IsReady)
                if (!string.IsNullOrEmpty(res))
                {
                    await Channel.SendMessageAsync(res);
                }
        }

        public async void LogF(string message)
        {
            Log(message);
            Flush();

        }

        private StringBuilder _buffer;

        public void Log(string message)
        {
            WriteMessage(message);
        }
    }
}
