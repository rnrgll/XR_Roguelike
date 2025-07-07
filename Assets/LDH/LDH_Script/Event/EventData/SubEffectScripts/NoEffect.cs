using UnityEngine;

namespace Event
{
    public class NoEffect : SubEffect
    {
        public NoEffect(SubEffectType subEffectType = SubEffectType.NoEffect, int value = 0, int? durationTurns = null) : base(subEffectType, value, durationTurns) { }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            Debug.Log("아무런 효과가 없음");
        }
    }
}