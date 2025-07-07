using Managers;
using System;
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

        public void ApplyEffects(Action onComplete)
        {
            int subEffectCount = SubEffectList.Count;
            if (subEffectCount == 0)
            {
                onComplete?.Invoke();
                return;
            }
            
            foreach (SubEffect subEffect in SubEffectList)
            {
                //서브 이팩트 효과 적용
                if (subEffect is GoldLossEffect && PenaltyCost!=null)
                {
                    int gap = Manager.GameState.Gold - subEffect.Value;
                    if (gap < 0)
                    {
                        var player = TurnManager.Instance.GetPlayerController();
                        player.ChangeMaxHp(-SubstituteCost??0 * gap);
                    }
                }
                subEffect.ApplyEffect(() =>
                {
                    subEffectCount--;
                    if(subEffectCount==0)
                        onComplete?.Invoke();
                });
            }
        }
    }
    
    
}