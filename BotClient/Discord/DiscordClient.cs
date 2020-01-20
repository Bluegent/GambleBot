﻿using System;
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
            _game = new GameController(this,_gen);

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
            if (message.Channel.Equals(Channel))
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

        public void LogF(string message)
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
