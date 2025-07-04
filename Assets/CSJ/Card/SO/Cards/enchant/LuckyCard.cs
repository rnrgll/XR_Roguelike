using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "LuckyCard", menuName = "Cards/Enchant/Lucky")]
public class LuckyCard : CardEnchantSO
{
    [SerializeField] int BonusScore = 60;
    [SerializeField] int BonusMoney = 50;
    public override void OnCardPlayed(MinorArcana card)
    {
        int rand = RandomManager.Instance.RandInt(0, 100);
        if (rand < 20)
        {
            controller.SetTurnBonusList(CardBonus.Mult, BonusType.Bonus, BonusScore);
        }
        else if (rand < 30)
        {
            GameStateManager.Instance.AddGold(BonusMoney);
        }
    }
}
