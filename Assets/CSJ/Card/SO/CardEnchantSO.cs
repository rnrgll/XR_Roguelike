using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardEnchantSO : ScriptableObject
{
    public CardEnchant EnchantType => _enchant;
    [SerializeField] private CardEnchant _enchant;
    [TextArea] public string description;

    /// <summary>
    /// 카드에 인챈트를 적용할 떄 호출
    /// </summary>
    public virtual void OnApply(MinorArcana card, CardController controller) { }

    /// <summary>
    /// 카드가 플레이될 때마다 호출
    /// </summary>
    public virtual void OnCardPlayed(MinorArcana card, CardController controller) { }

    /// <summary>
    /// 턴이 끝날 때 호출
    /// </summary>
    public virtual void OnTurnEnd(CardController controller) { }

    /// <summary>
    /// 최종 점수 계산 시 호출
    /// </summary>
    public virtual void GetBonus() { }
}