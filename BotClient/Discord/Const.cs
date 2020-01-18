namespace BotClient.Discord
{
    public class Const
    {
        public class Comm
        {
            public const string PLAY = "play";
            public static readonly string PLAY_HELP = $"`{PLAY}` - start playing the game with the current lobby";
            public const string LOBBY = "lobby";
            public static readonly string LOBBY_HELP = $"`{LOBBY}` - starts a lobby and joins it";
            public const string ROLL = "roll";
            public static readonly string ROLL_HELP = $"`{ROLL}` - rolls a new number";
            public const string RAISE = "raise";
            public static readonly string RAISE_HELP = $"`{RAISE}` - adds another standard bet sum to the current round's bet amount";
            public const string JOIN = "join";
            public static readonly string JOIN_HELP = $"`{JOIN}` - joins the current lobby";
            public const string SKIP = "skip";
            public static readonly string SKIP_HELP = $"`{SKIP}` - skip the current roll, however costs as much as the current round's cash out(current bet divided by player number)";
            public const string DEBUG = "debug";
            public static readonly string DEBUG_HELP = $"`{DEBUG}` - adds GambleBot as an AI player";
            public const string HELP = "help";
            public static readonly string HELP_HELP = $"`{HELP}` - prints this message";
            public const string ME = "me";
            public static readonly string ME_HELP = $"`{ME}` - prints your current stats";
        }

        public class Files
        {
            public const string DB = "players.json";
        }
        public class Emoji
        {
            public const string CURRENCY = "<:g_icon_own:667845967869116421>";
            public const string YAMERO = "<:yamero:601292524702662666>";
            public const string SAD = "<:sad:601292295026769931>";
            public const string WOKE = "<:woke:603122406927761408>";
            public const string KAPPA = "<:kappa:601295656044331019>";
        }
    }
}