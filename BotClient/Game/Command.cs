namespace BotClient.Game
{
    public class Command
    {
        public enum CommType
        {
            Roll, //roll
            Raise, // add to bed
            Fold, // don't roll
            Lobby, // start a lobby
            Join, //join a game that has been started
            Play, //start playing the game
            Debug, // add a debug player
            Help //prints all commands and how to use them
            , Me //gives you your stats
            , Register // make an account
            , BotStats // see the bot's stats
            , Backup // 
            , Restore //
        }
        public string Text;
        public string Help;
        public CommType Type;

        public Command(CommType type, string text, string help)
        {
            Text = text;
            Type = type;
            Help = help;
        }
    }
}