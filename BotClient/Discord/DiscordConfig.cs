using Newtonsoft.Json;

namespace BotClient.Discord
{


    class DiscordConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; set; }
    }
}