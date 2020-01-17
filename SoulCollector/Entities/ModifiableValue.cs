using System.Collections.Generic;

namespace SoulCollector.Entities
{
    public abstract class ModifiableValue
    {
        protected List<StatModifier> _modifiers;

        public ModifiableValue()
        {
            _modifiers = new List<StatModifier>();
        }

        protected abstract void Recalculate();

        public void AddModifier(StatModifier mod)
        {
            _modifiers.Add(mod);
            Recalculate();
        }

        public void RemoveModifier(StatModifier mod)
        {
            _modifiers.Remove(mod);
            Recalculate();

        }
    }
}