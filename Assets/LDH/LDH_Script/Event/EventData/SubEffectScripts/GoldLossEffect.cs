using Managers;
using System;

namespace Event
{
    public class GoldLossEffect : SubEffect
    {
        public GoldLossEffect(int value) : base(SubEffectType.ResourceGain, value) { }
        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(onComplete);

            Manager.GameState.AddGold(-Value);
        }
    }
}