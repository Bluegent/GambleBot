using System.Collections.Generic;
using SoulCollector.Entities;
using SoulCollector.SoulEffects;

namespace SoulCollector.Combat
{
    public class CombatStage
    {
        private readonly Entity _parent;
        private readonly List<IEffect> _effects;

        public CombatStage(Entity parent)
        {
            _parent = parent;
            _effects = new List<IEffect>();
        }

        public void AddEffect(IEffect effect)
        {
            _effects.Add(effect);
        }

        public void RemoveEffect(IEffect effect)
        {
            _effects.Remove(effect);
        }

        public void ExecuteEffects()
        {
            foreach (IEffect effect in _effects)
                effect.Apply(_parent.GetState());
        }

    }

}