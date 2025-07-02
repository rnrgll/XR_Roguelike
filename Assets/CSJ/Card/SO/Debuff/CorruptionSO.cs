using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEffect/Debuff/Corruption")]
public class CorruptionSO : CardDebuffSO
{
    private bool IsUsed;
    public override void OnApply(MinorArcana card, CardController controller)
    {
        IsUsed = false;
    }
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        IsUsed = true;
    }
    public override void OnTurnEnd(MinorArcana card, CardController controller)
    {
        if (!IsUsed)
        {
            controller.Deck.Debuff(card, CardDebuff.Rust);
        }
        else controller.Deck.DebuffClear(card);
    }
}
