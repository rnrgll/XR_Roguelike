using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "MultCard", menuName = "Cards/Enchant/Mult")]
public class MultCardSO : CardEnchantSO
{
    [SerializeField] private int Mult = 3;

    //TODO : 연계를 통해 배수 추가
    public override void OnCardPlayed(MinorArcana card)
    {
        controller.SetTurnBonusList(CardBonus.Mult, BonusType.Bonus, Mult);
    }
}
