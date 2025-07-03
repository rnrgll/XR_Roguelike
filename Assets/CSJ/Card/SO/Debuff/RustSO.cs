using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rust", menuName = "Cards/Debuff/Rust")]
public class RustSO : CardDebuffSO
{
    [SerializeField] private int penalty = 50;
    private Action<MinorArcana> playHandler;

    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
            Debug.Log($"[부식] {card.CardName} 사용 → 데미지 {penalty}만큼 감소");
            controller.AddPanelty(penalty);
            // 부식 효과는 사라지지 않고 계속 유지된다
    }
}
