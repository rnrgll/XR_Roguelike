using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "GlassCard", menuName = "Cards/Enchant/Glass")]
public class GlassCard : CardEnchantSO
{
    [SerializeField] private int AttackBuff = 2;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        controller.SetTurnBonusList(CardBonus.Ratio, BonusType.Bonus, AttackBuff);
        int rand = RandomManager.Instance.RandInt(0, 100);
        if (rand < 25)
        {
            OnRemove(card, controller);
        }
    }
    public override void OnRemove(MinorArcana card, CardController controller)
    {
        // controller.Deck.EnchantClear(card);
    }
}
