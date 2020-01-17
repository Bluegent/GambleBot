using SoulCollector.Entities;

namespace SoulCollector.Combat
{
    using System.Collections.Generic;

    public class BattleState
    {
        public Entity[] Allies;
        public Entity[] Enemies;
        public readonly Entity Source;
        public bool IsInBattle;
        public long NextAttackTick;

        public BattleTracker Taken;
        public BattleTracker Dealt;

        public BattleState(Entity source)
        {
            Source = source;
            Taken = new BattleTracker();
            Dealt = new BattleTracker();
            Reset();

        }

        public void Reset()
        {
            IsInBattle = false;
            Allies = null;
            Enemies = null;
            Taken.Reset();
            Dealt.Reset();
            NextAttackTick = 0;
        }
    }
}