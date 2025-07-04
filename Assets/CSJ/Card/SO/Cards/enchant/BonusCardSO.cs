using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using CardEnum;

[CreateAssetMenu(fileName = "BonusCard", menuName = "Cards/Enchant/Bonus")]
public class BonusCardSO : CardEnchantSO
{
    [SerializeField] private int Bonus = 40;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        controller.SetTurnBonusList(CardBonus.Score, BonusType.Bonus, Bonus);
    }
}
