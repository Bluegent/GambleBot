using System.Collections.Generic;
using SoulCollector.Combat;

namespace SoulCollector.Entities
{

    using GameControllers;
    using Output;

    using SoulCollector.SoulEffects;

    public abstract class Entity
    {
        protected CombatEvent _hitState;
        protected CombatEvent _attackState;
        protected CombatEvent _battleState;
        protected CombatEvent _healStage;

        public string Name;
        public ILogger Log;
        protected BattleState State;


        public void AddPostHitEffect(IEffect effect)
        {
            _attackState.Post.AddEffect(effect);
        }

        public BattleState GetState()
        {
            return State;
        }
        protected Entity(Dependencies dep)
        {
            Log = dep.Log;
            _hitState = new CombatEvent(this);
            _attackState = new CombatEvent(this);
            _battleState = new CombatEvent(this);
            _healStage = new CombatEvent(this);
            State = new BattleState(this);
            Name = "";

        }

        public void Heal(Entity source, int amount)
        {
            _healStage.Pre.ExecuteEffects();
            HealImpl(source,amount);
            _healStage.CombatStage.ExecuteEffects();
            _healStage.Post.ExecuteEffects();
        }

        public void EnterBattle(Entity[] allies, Entity[] enemies)
        {
            if (State.IsInBattle)
                return;
            State.Enemies = enemies;
            List<Entity> party = new List<Entity>(allies);
            party.Remove(this);
            State.Allies = party.ToArray();
            _battleState.Pre.ExecuteEffects();
            State.IsInBattle = true;
        }

        public void LeaveBattle()
        {
            if (!State.IsInBattle)
                return;
            _battleState.Post.ExecuteEffects();
            State.Reset();
        }

        public void Attack(Entity[] targets)
        {
            _attackState.Pre.ExecuteEffects();
            foreach (Entity target in targets)
            {
                AttackImpl(targets);
            }

            _attackState.CombatStage.ExecuteEffects();
            _attackState.Post.ExecuteEffects();
        }

        public void TakeDamage(DamageInstance damage, Entity source)
        {
            _hitState.Pre.ExecuteEffects();
            TakeDamageImpl(damage, source);
            _hitState.CombatStage.ExecuteEffects();
            _hitState.Post.ExecuteEffects();
        }

        protected abstract void AttackImpl(Entity[] targets);
        protected abstract void HealImpl(Entity source, int amount);
        protected abstract void TakeDamageImpl(DamageInstance damage, Entity source);
        public abstract void Update(long gameTick);
        public abstract bool IsAlive();
    }
}