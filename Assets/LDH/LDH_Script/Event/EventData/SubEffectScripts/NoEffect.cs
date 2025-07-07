using System;
using UnityEngine;

namespace Event
{
    public class NoEffect : SubEffect
    {
        public NoEffect(SubEffectType subEffectType = SubEffectType.NoEffect, int value = 0, int? durationTurns = null) : base(subEffectType, value, durationTurns) { }

        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(onComplete);
            Debug.Log("아무런 효과가 없음");
        }
    }
}