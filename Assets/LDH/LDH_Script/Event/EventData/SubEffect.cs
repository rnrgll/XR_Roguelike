using InGameShop;
using UnityEngine;

namespace Event
{
    public class SubEffect
    {
        public SubEffectType SubEffectType;
        public int Value;
        public int? DurationTurns;

        public SubEffect(SubEffectType subEffectType, int value, int? durationTurns)
        {
            SubEffectType = subEffectType;
            Value = value;
            DurationTurns = durationTurns;
        }

        public void ApplyEffect()
        {
            Debug.Log($"효과 적용 : {SubEffectType.ToString()}, {Value}, {DurationTurns??0}턴");
        }
    }
}