
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BotClient.Game.Serialize
{
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public partial class SerializablePlayer
    {
        [JsonProperty("Id")]
        public ulong Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("GlobalScore")]
        public long GlobalScore { get; set; }

        [JsonProperty("Wins")]
        public long Wins { get; set; }

        [JsonProperty("Losses")]
        public long Losses { get; set; }

        [JsonProperty("WinStreak")]
        public long WinStreak { get; set; }

        [JsonProperty("LossStreak")]
        public long LossStreak { get; set; }

        public Player Convert()
        {
            Player player = new Player(Id,Name);
            player.WinStreak = WinStreak;
            player.LossStreak = LossStreak;
            player.Wins = Wins;
            player.Losses = Losses;
            player.GlobalScore = GlobalScore;
            return player;

        }
    }

    public partial class SerializablePlayer
    {
        public static SerializablePlayer[] FromJson(string json) => JsonConvert.DeserializeObject<SerializablePlayer[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SerializablePlayer[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }


}
