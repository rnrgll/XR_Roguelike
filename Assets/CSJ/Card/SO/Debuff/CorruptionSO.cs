using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Corruption", menuName = "Cards/Debuff/Corruption")]
public class CorruptionSO : CardDebuffSO
{
    [SerializeField] private CardDebuffSO rustDebuff; // 부식 디버프 종류

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        Debug.Log($"[부패] {card.CardName} 사용됨 → 부패 해제");
        controller.Deck.DebuffClear(card);
    }

    public override void OnTurnEnd(MinorArcana card, CardController controller)
    {
        Debug.Log($"[부패] {card.CardName} 사용되지 않음 → Rust로 부식됨");
        controller.ApplyDebuff(card, rustDebuff); // RustSO 부여
    }
}
