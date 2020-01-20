using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BotClient.Game
{
    using System;
    using System.Linq;

    using BotClient.Discord;

    using global::Serialize;

    public class Database
    {
        private Dictionary<ulong, Player> _db;



        public bool LoadFromCompressed(string compressed)
        {
            try
            {
                Player[] players = JsonConvert.DeserializeObject<Player[]>(Compressor.DecompressString(compressed));
                if (players.Length == 0)
                {
                    return false;
                }
                _db = new Dictionary<ulong, Player>();
                foreach (Player player in players)
                {
                    AddPlayer(player);
                }
                Save(Const.Files.DB);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when deserializing: "+e.Message);
                return false;
            }
        }

        public string SaveToCompressed()
        {
            if (_db.Count == 0)
                return "";
            return Compressor.CompressString(Serialize.Serializer.ToJson(_db.Values.ToArray()));
        }

        public void Load(string path)
        {
            _db = new Dictionary<ulong, Player>();
            if (!File.Exists(path))
            {
                return;
            }
            Player[] players = JsonConvert.DeserializeObject<Player[]>(Utils.FileHandler.Read(path));
            foreach (Player player in players)
            {
                AddPlayer(player);
            }
        }

        public void AllocateSalary(long sum)
        {
            foreach (Player player in _db.Values)
            {
                player.GiveMinimumMoney(sum);
            }
            Save(Const.Files.DB);
        }

        public void Save(string path)
        {
            if (_db.Count == 0)
                return;
            File.WriteAllText(path, Serialize.Serializer.ToJson(_db.Values.ToArray()));

        }
        public Database()
        {
            _db = new Dictionary<ulong, Player>();
        }

        public void AddPlayer(Player player)
        {
            if (_db.ContainsKey(player.Id))
                return;
            _db.Add(player.Id, player);
        }

        public bool HasPlayer(ulong id)
        {
            return _db.ContainsKey(id);
        }

        public Player GetPlayer(ulong id)
        {
            return _db.ContainsKey(id) ? _db[id] : new Player();
        }
    }
}