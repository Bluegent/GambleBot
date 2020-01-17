namespace SoulCollector.Combat
{
    using System.Collections.Generic;
    using System.Linq;


    public class BattleTracker
    {
        private List<DamageInstance> _hits;

        public int TotalDamage;
        public int TotalActualDamage;

        public DamageInstance GetLastHit()
        {
            return _hits.Last();
        }
        public int HitCount()
        {
            return _hits.Count;
        }

        public BattleTracker()
        {
            _hits = new List<DamageInstance>();
            Reset();
        }

        public void AddHit(DamageInstance hit)
        {
            _hits.Add(hit);
            TotalDamage += hit.Amount;
            TotalActualDamage += hit.TrueAmount;
        }

        public void Reset()
        {
            _hits.Clear();
            TotalDamage = 0;
            TotalActualDamage = 0;
        }
    }
}