using InGameShop;
using UI.Event;

namespace Event
{
    public class SubEffect
    {
        public SubEffectType SubEffectType;
        public int Value;
        public int? DurationTurns;
        public bool HasItemReward;
        public ItemType? ItemType;
        public int? ItemCount;
    }
}