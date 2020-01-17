namespace BotClient.Game
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Player
    {
        public long Id { get; set; }

        public string Name { get; private set; }
        public long GlobalScore { get; set; }
        public long CurrentMoney { get; private set; }

        Player()
        {
            Reset();
        }

        public void Reset()
        {
            CurrentMoney = 0;
        }

        public long Lose(long sum)
        {
            if (CurrentMoney >= sum)
            {
                CurrentMoney -= sum;
                return sum;
            }
            else
            {
                long money = CurrentMoney;
                CurrentMoney = 0;
                return money;
            }
        }

        public void Reward(long sum)
        {
            CurrentMoney += sum;
        }

        public void Win()
        {
            GlobalScore += CurrentMoney;
            CurrentMoney = 0;
        }
        public string ToString()
        {
            return $"{Name}({CurrentMoney} $)";
        }
    }


    public interface ILogger
    {
        void Log(string message);
    }


    public class GameConfig
    {
        public long StartMoney;

        public long BetStartMoney;

        public int StartRoll;

    }
    public class GambleGame
    {
        public enum Move
        {
            Roll,
            Raise
        }
        private List<Player> players;

        private ILogger _log;

        private Random _generator;

        private GameConfig gcfg;
        public long CurrentRoundBetMoney { get; private set; }

        private int _currentPlayer;

        private int _currentRoll;

        private bool _ongoing;

        public GambleGame(GameConfig cfg, Random gen, ILogger log)
        {
            this._log = log;
            _generator = gen;
            players = new List<Player>();
            gcfg = cfg;
            CurrentRoundBetMoney = cfg.BetStartMoney;
            _currentPlayer = 0;
            _currentRoll = cfg.StartRoll;
            _ongoing = true;
        }
        public Player GetCurrentPlayer()
        {
            return players[_currentPlayer];
        }
        private bool HasPlayers()
        {
            int playerCount = 0;
            foreach (Player player in players)
            {
                if (player.CurrentMoney > 0)
                    ++playerCount;
            }

            if (playerCount >= 2)
                return true;
            return false;
        }

        public void SetUpRound()
        {
            CurrentRoundBetMoney = gcfg.BetStartMoney;
            _currentPlayer = 0;
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
            for (int i=0;i<players.Count;++i)
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
        }

        public void Roll()
        {
            int roll = 1+_generator.Next(_currentRoll);
            //TODO: log that a roll has been performed
            _currentRoll = roll;
            
        }

        public void CheckEnd()
        {
            if (_currentRoll == 1)
            {
                //current player loses
                long money = players[_currentPlayer].Lose(CurrentRoundBetMoney);
                long playerShare = money / (players.Count - 1);
                for (int i = 0; i < players.Count; ++i)
                {
                    if (i != _currentPlayer)
                    {
                        //TODO: log that a player was rewarded
                        players[i].Reward(playerShare);
                    }
                }
                RemoveLosers();
                SetUpRound();
            }
        }

        public void EndGame()
        {
            _ongoing = false;
            players[0].Win();
            //TODO: Log that player has won
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
                players.Remove(player);
            }

            if (players.Count == 1)
            {
                EndGame();
            }
        }

        public void NextMove(Move move)
        {
            if (!_ongoing)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, null);
            }
            CheckEnd();
            ++_currentPlayer;
        }

        public void StartRound()
        {
            foreach (Player player in players)
            {
                player.Reset();
            }
            SetUpRound();
        }
    }
}