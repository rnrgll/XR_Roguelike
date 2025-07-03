using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LuckyCard", menuName = "Cards/Enchant/Lucky")]
public class LuckyCard : CardEnchantSO
{
    [SerializeField] int BonusScore = 60;
    [SerializeField] int BonusMoney = 50;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        int rand = RandomManager.Instance.RandInt(0, 100);
        if (rand < 20)
        {
            TurnManager.Instance.GetPlayerController().ApplyAttackBuff(BonusScore, 1);
        }
        else if (rand < 30)
        {
            GameStateManager.Instance.AddGold(BonusMoney);
        }
    }
}
