namespace SoulCollector.Output
{
    using SoulCollector.Entities;

    public class BattleLog
    {
        public bool Silent { get; set; }

        private Entity _parent;

        public BattleLog(Entity parent)
        {
            _parent = parent;
        }
    }
}