using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "GoldCard", menuName = "Cards/Enchant/Gold")]
public class GoldCard : CardEnchantSO
{
    [SerializeField] int Money = 100;
    [SerializeField] int Damage = 100;

    public override void OnCardPlayed(MinorArcana card)
    {
        controller.SetTurnBonusList(CardBonus.Score, BonusType.Bonus, Damage);
    }
    public override void OnTurnEnd(MinorArcana card)
    {
        GameStateManager.Instance.AddGold(Money);
    }
}
