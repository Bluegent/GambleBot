
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

    public static class Serializer
    {
        public static string ToJson(Player[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
