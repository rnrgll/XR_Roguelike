using Event;

namespace Event
{
    public class EventMainReward
    {
        public int Id;
        public string Text;
        public EffectType RewardType;
        public int RewardEffectID;

        public EventMainReward(int id, string text, EffectType rewardType, int rewardEffectID)
        {
            Id = id;
            Text = text;
            RewardType = rewardType;
            RewardEffectID = rewardEffectID;
        }
    }
    
}