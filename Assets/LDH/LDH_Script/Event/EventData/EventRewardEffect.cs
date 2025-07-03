using Managers;
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

        public void ApplyEffects()
        {
            foreach (SubEffect subEffect in SubEffectList)
            {
                //서브 이팩트 효과 적용
                subEffect.ApplyEffect();
                
            }
            
            //패널티가 있으면 패널티 적용
            if (PenaltyCost != null)
            {
                if(Manager.GameState.)
            }
        }
    }
    
    
}