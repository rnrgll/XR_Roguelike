using Managers;
using System;

namespace Event
{
    public class GoldGainEffect : SubEffect
    {
        public GoldGainEffect(int value) : base(SubEffectType.ResourceGain, value) { }

        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(onComplete);
            Manager.GameState.AddGold(Value);
        }
    }
}