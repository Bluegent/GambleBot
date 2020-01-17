using SoulCollector.Entities;

namespace SoulCollector.SoulEffects
{
    using SoulCollector.Combat;

    public interface IEffect
    {
        void Apply(BattleState state);
    }
}