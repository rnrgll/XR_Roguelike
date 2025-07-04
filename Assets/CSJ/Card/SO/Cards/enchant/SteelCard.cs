using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "SteelCard", menuName = "Cards/Enchant/Steel")]
public class SteelCard : CardEnchantSO
{
    [SerializeField] private int MultPlayed = 3;
    [SerializeField] private float MultInHand = 1.5f;

    public override void OnSubscribe(MinorArcana card)
    {
        base.OnSubscribe(card);
        controller.SetTurnBonusList(CardBonus.Ratio, BonusType.Bonus, MultInHand);
    }

    public override void OnCardPlayed(MinorArcana card)
    {
        controller.SetTurnBonusList(CardBonus.Ratio, BonusType.Bonus, MultPlayed);
        OnUnSubscribe(card);
    }


}
