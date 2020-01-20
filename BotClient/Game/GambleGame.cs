using BotClient.Discord;

namespace BotClient.Game
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface ILogger
    {
        void Log(string message);
        void Flush();
        void LogF(string message);
    }

    public class GambleGame
    {
        public enum Move
        {
            Roll,
            Raise,
            Skip
        }
        private List<Player> players;

        private ILogger _log;

        private Random _generator;

        private GameConfig gcfg;
        public long CurrentRoundBetMoney { get; private set; }

        private int _currentPlayer;

        private int _currentRoll;

        public bool Finished { get; private set; }


        public GambleGame(GameConfig cfg, Random gen, ILogger log)
        {
            this._log = log;
            _generator = gen;
            players = new List<Player>();
            gcfg = cfg;
            CurrentRoundBetMoney = cfg.BetStartMoney;
            _currentPlayer = 0;
            _currentRoll = cfg.StartRoll;
            Finished = false;
        }
        public Player GetCurrentPlayer()
        {
            if(players.Count  > _currentPlayer)
                return players[_currentPlayer];
            return new Player();
        }

        public void SetUpRound()
        {
            if (Finished)
                return;
            CurrentRoundBetMoney = gcfg.BetStartMoney;
            _currentPlayer = _generator.Next(players.Count);
            _currentRoll = gcfg.StartRoll;
            _log.Log($"A new round has started! Current bet is {CurrentRoundBetMoney} {Const.Emoji.CURRENCY}. {players[_currentPlayer].Tag()}, roll!");
            FakePlayerMove();
        }

        public void AddPlayer(Player player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }
        }

        public string GetPlayerList()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < players.Count; ++i)
            {
                sb.Append(players[i]);
                if (i <= players.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }

        public void RaiseBet()
        {
            CurrentRoundBetMoney += gcfg.BetStartMoney;
            _log.Log($"Bet has been raised to  {CurrentRoundBetMoney} {Const.Emoji.CURRENCY} by {players[_currentPlayer].Name}. {Const.Emoji.YAMERO}");
        }

        public void Roll()
        {
            int roll = players[_currentPlayer].Roll(_currentRoll, _generator);
            //TODO: log that a roll has been performed
            _log.Log($"{players[_currentPlayer]} rolled >>>**{roll}**<<< !");
            _currentRoll = roll;

        }

        public bool CheckEnd()
        {
            if (_currentRoll == 1)
            {
                //current player loses

                long money = players[_currentPlayer].Lose(CurrentRoundBetMoney);
                _log.Log($"{players[_currentPlayer]} rolled a 1 and lost {money} {Const.Emoji.CURRENCY}. {Const.Emoji.SAD}");
                long playerShare = money / (players.Count - 1);
                StringBuilder winners = new StringBuilder();
                for (int i = 0; i < players.Count; ++i)
                {
                    if (i != _currentPlayer)
                    {
                        //TODO: log that a player was rewarded
                        players[i].Reward(playerShare);
                        winners.Append(players[i] + " ");
                    }

                }
                winners.Append(players.Count == 2 ? $" gains {playerShare} {Const.Emoji.CURRENCY}." : $" gain {playerShare} {Const.Emoji.CURRENCY}.");
                _log.Log(winners.ToString());
                RemoveLosers();
                SetUpRound();
                return true;
            }
            return false;
        }

        public void EndGame()
        {
            Finished = true;
            players[0].Win();
            //TODO: Log that player has won
            _log.Log($"{players[0].Name} has won! Global score: {players[0].GlobalScore} {Const.Emoji.CURRENCY}. {Const.Emoji.WOKE}");
            long win = _generator.Next(3);
            if (win == 0)
            {
                long sum = gcfg.LootBoxMin +_generator.Next( gcfg.LootBoxMax - gcfg.LootBoxMin + 1);
                _log.Log($"{players[0].Name} has found a lucky chip worth {sum} {Const.Emoji.CURRENCY}!");
                players[0].Lootbox(sum);
            }
        }

        public void ResetPlayerIndex(int mod = 0)
        {
            _currentPlayer += mod;
            if (_currentPlayer >= players.Count)
                _currentPlayer = 0;
            if (_currentPlayer < 0)
                _currentPlayer = 0;

        }
        public void RemoveLosers()
        {
            List<Player> toRemove = new List<Player>();
            foreach (Player player in players)
            {
                if (player.CurrentMoney == 0)
                {
                    toRemove.Add(player);
                }
            }

            foreach (Player player in toRemove)
            {
                //TODO: log that a player has been removed
                _log.Log($"{player.Name} is broke so they have been removed from the game. {Const.Emoji.SAD}");
                players.Remove(player);
                player.Lost();
            }

            if (players.Count == 1)
            {
                EndGame();
            }
             else if (toRemove.Count != 0)
            {
                ResetPlayerIndex(-1);
            }

        }

        public void FakePlayerMove()
        {
            if (players[_currentPlayer].Id != 0)
                return;
            if (players[_currentPlayer].CurrentMoney >= gcfg.StartMoney * 0.6 && _generator.Next(100) < 10 &&
                _currentRoll > 10)
            {
                NextMove(Move.Raise);
            }
            else
            {
                NextMove(Move.Roll);
            }
        }

        public long Cashout()
        {
            return CurrentRoundBetMoney / (players.Count - 1);
        }
        public void Skip()
        {
            players[_currentPlayer].Lose(Cashout());
            _log.Log($"{players[_currentPlayer]} has chosen to skip his turn. {Const.Emoji.KAPPA}");
            RemoveLosers();

        }
        public void NextMove(Move move)
        {
            if (Finished)
                return;

            switch (move)
            {
                case Move.Roll:
                    Roll();
                    break;
                case Move.Raise:
                    RaiseBet();
                    Roll();
                    break;
                case Move.Skip:
                    Skip();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, null);
            }
            if(CheckEnd())
                return;
            if (Finished)
                return;
            ResetPlayerIndex(1);
            _log.Log($"{players[_currentPlayer].Tag()} is next to move, cashout is {Cashout()}.");
            FakePlayerMove();

        }

        public void Start()
        {
            foreach (Player player in players)
            {
                player.StartMatch(gcfg.StartMoney);
            }
            _log.Log($"Game has started. Players: {GetPlayerList()}.");
            SetUpRound();

        }

        public bool CanBegin()
        {
            return players.Count >= 2;
        }
    }
}