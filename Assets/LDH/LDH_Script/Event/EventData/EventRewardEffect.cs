using System.Collections.Generic;
using System.Linq;

namespace Event
{
    
    public class EventRewardEffect
    {
        public int RewardEffectID;
        public List<SubEffect> SubEffectList;
        public int? PenaltyCost;
        public int? SubstituteCost;
        public List<ItemRewardType> ItemRewards;
    }
    
    
}