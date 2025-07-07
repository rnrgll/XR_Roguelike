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
                if (subEffect is GoldLossEffect && PenaltyCost!=null)
                {
                    int gap = Manager.GameState.Gold - subEffect.Value;
                    if (gap < 0)
                    {
                        //todo : 테스트 필요 
                        var player = TurnManager.Instance.GetPlayerController();
                        player.ChangeMaxHp(-SubstituteCost??0 * gap);
                    }
                }
                subEffect.ApplyEffect();
            }
        }
    }
    
    
}