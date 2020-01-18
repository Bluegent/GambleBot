using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using BotClient.Game.Serialize;
using Newtonsoft.Json;

namespace BotClient.Game
{
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
            SerializablePlayer[] splayers = JsonConvert.DeserializeObject<SerializablePlayer[]>(Utils.FileHandler.Read(path));
            foreach (SerializablePlayer player in splayers)
            {
                AddPlayer(player.Convert());
            }
        }

        public void Save(string path)
        {
            if (_db.Count == 0)
                return;
            List<SerializablePlayer> toSave = new List<SerializablePlayer>();
            foreach (Player player in _db.Values)
            {
                toSave.Add(player.Convert());
            }
            File.WriteAllText(path, Serialize.Serialize.ToJson(toSave.ToArray()));

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