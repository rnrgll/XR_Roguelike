using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Corruption", menuName = "Cards/Debuff/Corruption")]
public class CorruptionSO : CardDebuffSO
{
    private bool isUsedThisTurn;
    [SerializeField] private CardDebuff rustDebuff; // 부식 디버프 종류

    public override void OnSubscribe(MinorArcana card, CardController controller)
    {
        isUsedThisTurn = false;
        base.OnSubscribe(card, controller);
    }

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        isUsedThisTurn = true;
    }

    public override void OnTurnEnd(MinorArcana card, CardController controller)
    {
        if (!isUsedThisTurn)
        {
            Debug.Log($"[부패] {card.CardName} 사용되지 않음 → Rust로 부식됨");
            controller.Deck.Debuff(card, rustDebuff); // RustSO 부여
        }
        else
        {
            Debug.Log($"[부패] {card.CardName} 사용됨 → 부패 해제");
            controller.Deck.DebuffClear(card);
        }

        isUsedThisTurn = false;
    }
}
