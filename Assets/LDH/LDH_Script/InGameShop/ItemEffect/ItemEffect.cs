using System;

namespace Item
{
    [Serializable]
    public class ItemEffect
    {
        public EffectType effectType;
        public float percentValue;
        public int value;
        public int duration;     // 지속 턴 수
    }
}