using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using BotClient.Discord;
using Discord;
using Discord.WebSocket;

namespace BotClient.Game
{
    using System.Runtime.Serialization;

    public class GameController
    {

        public enum GameState
        {
            None,
            JoinPhase,
            OngoingPhase
        }
        public Database Db { get; private set; }
        private GambleGame _game;
        private ILogger _log;
        private GameConfig _gcfg;
        private Random _gen;
        public GameState State;
        private bool _fakePlayer;
        private Dictionary<string, Command> _commands;
        public string HelpMessage;
        public void Reset()
        {
            _fakePlayer = false;
            _game = new GambleGame(_gcfg, _gen, _log);
            State = GameState.None;
        }


        public void AllocateSalary()
        {
            Db.AllocateSalary(_gcfg.StartMoney);
            _log.LogF("Everyone has been allocated their _deserved_ wages.");
        }

        public void StartGame(SocketUser user)
        {
            if (State != GameState.None)
            {
                //TODO: log game already in progress
                _log.LogF($"A game is already in progress or a lobby has already been started.");
                return;
            }
            _log.LogF($"A lobby has been started. Join by typing `{Const.Comm.JOIN}` or start playing the game by typing `{Const.Comm.PLAY}`.");
            _log.Flush();
            State = GameState.JoinPhase;
            JoinGame(user);
        }

        public void BeginGame()
        {
            if (State != GameState.JoinPhase)
            {
                //TODO: log can't begin if no room was started
                _log.LogF($"A game can't be started without a lobby. Start one by typing `{Const.Comm.LOBBY}`.");
                return;
            }
            if (!_game.CanBegin())
            {
                _log.LogF($"A game can't be started without at least 2 players. Joing by typing `{Const.Comm.JOIN}` or add a fake player by typing `{Const.Comm.DEBUG}`.");
                //TODO: log can't begin with less than 2 players
                return;
            }

            State = GameState.OngoingPhase;
            _game.Start();

        }

        public void AddFakePlayer()
        {
            if (_fakePlayer)
                return;
            _log.LogF("A fake player has been added.");
            _fakePlayer = true;
            Player fake;
            if (Db.HasPlayer(0))
            {
                fake = Db.GetPlayer(0);
            }
            else
            {
                fake = new Player(0, "GambleBot");
                Db.AddPlayer(fake);
            }
            fake.GiveMinimumMoney(_gcfg.StartMoney);
            _game.AddPlayer(fake);
        }
        public void JoinGame(SocketUser user)
        {
            if (State != GameState.JoinPhase)
            {
                _log.LogF($"A game is either in progress or no lobby exists. Start a lobby with `{Const.Comm.LOBBY}` or wait for the current game to end.");
                return;
            }

            Player player;
            if (!Db.HasPlayer(user.Id))
            {
                Register(user);
                
            }
            player = Db.GetPlayer(user.Id);

            if (!player.CanJoin(_gcfg.StartMoney))
            {
                _log.LogF($"{player.Name} is too poor to join, laugh at him! {Const.Emoji.KAPPA}");
                return;
            }
            _log.LogF($"{player.Name} has joined the lobby.");
            _game.AddPlayer(player);
        }

        public void DoMove(GambleGame.Move move)
        {
            if (State != GameState.OngoingPhase)
            {
                //TODO: log can't move when game is not ongoing
                _log.LogF($"No game is in progress. Type `{Const.Comm.LOBBY}` to start a lobby or `{Const.Comm.PLAY}` to start playing with the current lobby.");
                return;
            }
            _game.NextMove(move);
        }


        private void AddCommand(Command.CommType type, string text, string help)
        {
            Command comm = new Command(type, text,help);
            _commands.Add(comm.Text, comm);
        }

        public GameController(ILogger log, Random gen)
        {
            _log = log;
            _gen = gen;
            _gcfg = new GameConfig();
            _gcfg.BetStartMoney = 10;
            _gcfg.StartMoney = 50;
            _gcfg.StartRoll = 1000;
            _gcfg.LootBoxMin = 10;
            _gcfg.LootBoxMax = 50;
            Db = new Database();
            Db.Load(Const.Files.DB);
            State = GameState.None;

            Reset();


            _commands = new Dictionary<string, Command>();

            AddCommand(Command.CommType.Play, Const.Comm.PLAY,Const.Comm.PLAY_HELP);
            AddCommand(Command.CommType.Lobby, Const.Comm.LOBBY, Const.Comm.LOBBY_HELP);
            AddCommand(Command.CommType.Join, Const.Comm.JOIN, Const.Comm.JOIN_HELP);
            AddCommand(Command.CommType.Roll, Const.Comm.ROLL, Const.Comm.LOBBY_HELP);
            AddCommand(Command.CommType.Raise, Const.Comm.RAISE, Const.Comm.RAISE_HELP);
            AddCommand(Command.CommType.Debug, Const.Comm.DEBUG, Const.Comm.DEBUG_HELP);
            AddCommand(Command.CommType.Fold, Const.Comm.SKIP, Const.Comm.SKIP_HELP);
            AddCommand(Command.CommType.Help, Const.Comm.HELP, Const.Comm.HELP_HELP);
            AddCommand(Command.CommType.Me, Const.Comm.ME, Const.Comm.ME_HELP);
            AddCommand(Command.CommType.Register, Const.Comm.REGISTER, Const.Comm.REGISTER_HELP);
            AddCommand(Command.CommType.BotStats, Const.Comm.BOT_STATS, Const.Comm.BOT_STATS_HELP);
            GetHelpMessage();
        }

        private void GetHelpMessage()
        {
            StringBuilder help = new StringBuilder();
            help.Append($"``` Hello. I'm GableBot and I run a high stake gambling operation. The currently available game is Death Roll. If your vault has enough cash({_gcfg.StartMoney}), you can join. Everyone is issued {_gcfg.StartMoney} tokens. A random player is chosen to roll a number between 1 and {_gcfg.StartRoll}. The next player will then roll between 1 and the previous number, and so on until a player rolls a 1, losing. The amount of money they bet is then equally distributed between the remaining players and the next round starts. Running out of money removes a player from the game. The winner gets their final money added to their score and vault. Don't worry, if you're broke, every 30 minutes everyone is allocated enough tokens to play once. To interact with me use the following commands: \n");
            foreach (Command command in _commands.Values)
            {
                help.Append(command.Help);
                help.Append("\n");
            }
            help.Append("```");
            HelpMessage = help.ToString();

        }

        public bool Register(SocketUser user)
        {
            if (Db.HasPlayer(user.Id))
            {
                return false;
            }
            else
            {
                Player player = new Player(user.Id, user.Username);
                player.GiveMinimumMoney(_gcfg.StartMoney);
                Db.AddPlayer(player);
                return true;
            }
        }

        public bool IsWrongUser(ulong id)
        {
            if (State != GameState.OngoingPhase)
            {
                _log.LogF("Cool your jets, kid, there's no game in progress.");
                return true;
            }
            else if (id != _game.GetCurrentPlayer().Id)
            {
                _log.LogF("It's not your turn, punk.");
                return true;
            }

            return false;
        }

        public void HandleMessage(SocketMessage msg)
        {
            string[] msgBits = msg.Content.Trim().Split(' ');

            string cmdText = msgBits[0].ToLower().Trim();
            Command cmd = _commands.ContainsKey(cmdText) ? _commands[cmdText] : null;

            if (cmd == null)
                return;
            switch (cmd.Type)
            {
                case Command.CommType.Roll:
                    if(IsWrongUser(msg.Author.Id)){ 
                        return;
                    }
                    DoMove(GambleGame.Move.Roll);
                    break;
                case Command.CommType.Raise:
                    if (IsWrongUser(msg.Author.Id))
                    {
                        return;
                    }
                    DoMove(GambleGame.Move.Raise);
                    break;
                case Command.CommType.Fold:
                    if (IsWrongUser(msg.Author.Id))
                    {
                        return;
                    }
                    DoMove(GambleGame.Move.Skip);
                    break;
                case Command.CommType.Lobby:
                    StartGame(msg.Author);
                    break;
                case Command.CommType.Join:
                    JoinGame(msg.Author);
                    break;
                case Command.CommType.Play:
                    BeginGame();
                    break;
                case Command.CommType.Debug:
                    AddFakePlayer();
                    break;
                case Command.CommType.Help:
                    msg.Author.SendMessageAsync(HelpMessage);
                    break;
                case Command.CommType.Register:
                    if (Db.HasPlayer(msg.Author.Id))
                    {
                        _log.LogF($"I already know you: {Db.GetPlayer(msg.Author.Id).Full()}");
                    }
                    else
                    {
                        Register(msg.Author);
                    }
                    break;
                case Command.CommType.Me:
                {
                    if (!Db.HasPlayer(msg.Author.Id))
                    {
                        _log.LogF("Player doesn't exist in database, start playing to register.");
                        break;
                    }

                    _log.LogF(Db.GetPlayer(msg.Author.Id).Full());
                    break;
                }
                case Command.CommType.BotStats:
                    _log.LogF(Db.GetPlayer(0).Full());
                    break;
            }
            _log.Flush();
            if (State == GameState.OngoingPhase && _game.Finished)
            {
                Db.Save(Const.Files.DB);
                Reset();
            }


        }
    }
}