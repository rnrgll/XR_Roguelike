using InGameShop;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    public abstract class SubEffect
    {
        public SubEffectType SubEffectType;
        public int Value;
        public int? DurationTurns;

        public SubEffect(SubEffectType subEffectType = SubEffectType.NoEffect, int value = 0, int? durationTurns = null)
        {
            SubEffectType = subEffectType;
            Value = value;
            DurationTurns = durationTurns;
        }

        public virtual void ApplyEffect()
        {
            Debug.Log($"효과 적용 : {SubEffectType.ToString()}, {Value}, {DurationTurns ?? 0}턴");
        }
    }
}