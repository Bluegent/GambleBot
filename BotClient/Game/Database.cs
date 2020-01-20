using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BotClient.Game
{
    using System.Linq;

    using BotClient.Discord;

    public class Database
    {
        private Dictionary<ulong, Player> _db;


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
            if(_db.ContainsKey(player.Id)) 
                return;
            _db.Add(player.Id,player);           
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