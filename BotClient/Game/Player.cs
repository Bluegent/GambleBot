using System;
using BotClient.Discord;
using BotClient.Game.Serialize;

namespace BotClient.Game
{
    using System.Threading;

    using global::Discord.Rest;

    using Newtonsoft.Json;

    public class Player
    {
        public long CurrentMoney { get; private set; }

        [JsonProperty("Id")]
        public ulong Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("GlobalScore")]
        public long GlobalScore { get; set; }

        [JsonProperty("GlobalMoney")]
        public long GlobalMoneyRead {
            get
            {
                long money;
                lock (_moneyMutex)
                {
                    money = _globalMoney;
                }

                return money;
            }
            set
            {
                lock (_moneyMutex)
                {
                    _globalMoney = value;
                }
            }
        }

        private long _globalMoney;

        [JsonProperty("Wins")]
        public long Wins { get; set; }

        [JsonProperty("Losses")]
        public long Losses { get; set; }

        [JsonProperty("WinStreak")]
        public long WinStreak { get; set; }

        [JsonProperty("LossStreak")]
        public long LossStreak { get; set; }

        private static Mutex _moneyMutex = new Mutex();


        public void GiveMinimumMoney(long min)
        {
            lock (_moneyMutex)
            {
                if (_globalMoney < min)
                {
                    _globalMoney = min;
                }
            }
        }

        public bool CanJoin(long cost)
        {
            bool result = false;
            lock (_moneyMutex)
            {
                result = _globalMoney >= cost;
            }

            return result;
        }

        public void StartMatch(long cost)
        {
            lock (_moneyMutex)
            {
                _globalMoney -= cost;
                CurrentMoney = cost;
            }
        }

        public Player()
        {
            CurrentMoney = 0;
        }

        public void Lost()
        {
            ++Losses;
            ++LossStreak;
            WinStreak = 0;
        }

        public Player(ulong id, string name)
        {
            Name = name;
            Id = id;
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


        public string Tag()
        {
            if (Id == 0)
                return "<@!667751728338173991>";
            return $"<@!{Id}>";
        }
        public void Reward(long sum)
        {
            CurrentMoney += sum;
        }

        public void Lootbox(long sum)
        {
            lock (_moneyMutex)
            {
                _globalMoney += sum;
            }
        }

        public void Win()
        {
            ++Wins;
            ++WinStreak;
            LossStreak = 0;
            GlobalScore += CurrentMoney;
            lock (_moneyMutex)
            {
                _globalMoney += CurrentMoney;
            }
            CurrentMoney = 0;
        }

        public override string ToString()
        {
            return $"{Name}[{CurrentMoney} {Const.Emoji.CURRENCY}]";
        }

        public string Full()
        {
            return $"{Name}(Score: {GlobalScore}, Vault: {_globalMoney}{Const.Emoji.CURRENCY})[{Wins} {(Wins == 1 ? "Win" : "Wins")} : {Losses} {(Losses == 1 ? "Loss" : "Losses")}, Win Streak: {WinStreak}, Loss Streak: {LossStreak}]";
        }
        public int Roll(int max, Random gen)
        {
            return 1 + gen.Next(max);
        }


        public static Player[] FromJson(string json) => JsonConvert.DeserializeObject<Player[]>(json, Converter.Settings);
    }
}