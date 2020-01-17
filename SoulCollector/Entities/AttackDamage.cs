using SoulCollector.Combat;

namespace SoulCollector.Entities
{
    public class AttackDamage
    {
        public Stat Min { get; }
        public Stat Max { get; }
        public DamageType Type { get; }

        public AttackDamage(DamageType type, int baseMin, int baseMax)
        {
            Min = new Stat(StatType.Attack, baseMin);
            Max = new Stat(StatType.Attack, baseMax);
            Type = type;
        }

        public int Get()
        {
            if (Min.Value >= Max.Value)
            {
                return Max;
            }
            return Utils.Math.Random(Min, Max);
        }

        public DamageInstance GetDamageInstance()
        {
            DamageInstance result = new DamageInstance();
            result.Amount = Get();
            result.Type = Type;
            return result;
        }
    }

}