using SoulCollector.Entities;

namespace SoulCollector.Combat
{
    public class CombatEvent
    {
        public CombatStage Pre { get; }
        public CombatStage CombatStage { get; }
        public CombatStage Post { get; }

        public CombatEvent(Entity parent)
        {
            Pre = new CombatStage(parent);
            CombatStage = new CombatStage(parent);
            Post = new CombatStage(parent);
        }

    }
}