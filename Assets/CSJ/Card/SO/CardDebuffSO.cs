using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardDebuffSO : ScriptableObject
{
    public CardDebuff DebuffType => _type;
    [SerializeField] private CardDebuff _type;
    [TextArea] public string description;

    /// <summary>
    /// 디버프가 카드에 걸릴 때(Setup 직후) 한 번 호출
    /// </summary>
    public virtual void OnApply(MinorArcana card, CardController controller) { }

    /// <summary>
    /// 카드가 플레이될 때마다 호출
    /// </summary>
    public virtual void OnCardPlayed(MinorArcana card, CardController controller) { }

    /// <summary>
    /// 턴이 끝날 때 호출
    /// </summary>
    public virtual void OnTurnEnd(MinorArcana card, CardController controller) { }

    /// <summary>
    /// 배틀이 끝날 때 호출하여 디버프들을 제거
    /// </summary>
    public void OnBattleEnd(MinorArcana card, CardController controller)
    {
        controller.Deck.DebuffClear(card);
    }
}