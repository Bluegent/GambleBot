namespace SoulCollector.SoulEffects
{
    using SoulCollector.Combat;
    using SoulCollector.Entities;
    using SoulCollector.GameControllers;
    using SoulCollector.Output;

    public class LifeStealEffect : IEffect
    {
        private Entity _parent;
        private ILogger _log;
        public LifeStealEffect(Dependencies dep, Entity parent)
        {
            _log = dep.Log;
            _parent = parent;
        }
        public void Apply(BattleState state)
        {
            DamageInstance lastHit = state.Dealt.GetLastHit();
            int amount = (int)( 0.2* lastHit.TrueAmount);
            _parent.Heal(_parent,amount);
            _log.Log($"{_parent} was healed for {amount}(20% lifesteal).");
        }
    }
}