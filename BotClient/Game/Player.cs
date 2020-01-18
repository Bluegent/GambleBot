using System;
using BotClient.Discord;
using BotClient.Game.Serialize;

namespace BotClient.Game
{
    public class Player
    {
        public ulong Id { get; set; }

        public string Name { get; private set; }
        public long GlobalScore { get; set; }
        public long CurrentMoney { get; private set; }
        public long Wins;
        public long Losses;
        public long WinStreak;
        public long LossStreak;


        public SerializablePlayer Convert()
        {
            SerializablePlayer player = new SerializablePlayer();
            player.Name = Name;
            player.Id = Id;
            player.Wins = Wins;
            player.LossStreak = LossStreak;
            player.Losses = Losses;
            player.WinStreak = WinStreak;
            player.GlobalScore = GlobalScore;
            return player;
        }
        public Player()
        {
            Reset(0);
        }

        public Player(ulong id, string name)
        {
            Name = name;
            Id = id;
        }

        public void Reset(long sum)
        {
            CurrentMoney = sum;
        }

        public long Lose(long sum)
        {
            ++Losses;
            ++LossStreak;
            WinStreak = 0;

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

        public void Win()
        {
            ++Wins;
            ++WinStreak;
            LossStreak = 0;
            GlobalScore += CurrentMoney;
            CurrentMoney = 0;
        }

        public override string ToString()
        {
            return $"{Name}({CurrentMoney} {Const.Emoji.CURRENCY})";
        }

        public string Full()
        {
            return $"(Global Score: {GlobalScore} {Const.Emoji.CURRENCY}){Name} [ {Wins} {(Wins == 1 ? "Win" : "Wins")} : {Losses} {(Losses == 1 ? "Loss" : "Losses")}, Win Streak: {WinStreak}, Loss Streak: {LossStreak}]";
        }
        public int Roll(int max, Random gen)
        {
            return 1 + gen.Next(max);
        }
    }
}