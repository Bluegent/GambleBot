namespace SoulCollector.Combat
{
    public enum DamageType
    {
        Physical,
        Fire,
        Water,
        Earth,
        Light,
        Darkness,
        True
    }
    public class DamageInstance
    {
        public int Amount;
        public int TrueAmount;
        public DamageType Type;

        public override string ToString()
        {
            return $"{TrueAmount} {Type} Damage";
        }
    }
}