using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlassCard", menuName = "Cards/Enchant/Glass")]
public class GlassCard : CardEnchantSO
{
    [SerializeField] private int AttackBuff = 2;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        TurnManager.Instance.GetPlayerController().ApplyAttackBuff(AttackBuff, 1);
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
