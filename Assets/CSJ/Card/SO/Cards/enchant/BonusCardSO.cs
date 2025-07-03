using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusCard", menuName = "Cards/Enchant/Bonus")]
public class BonusCardSO : CardEnchantSO
{
    [SerializeField] private int Bonus = 40;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        TurnManager.Instance.GetPlayerController().ApplyFlatAttackBuff(Bonus, 1);
    }
}
