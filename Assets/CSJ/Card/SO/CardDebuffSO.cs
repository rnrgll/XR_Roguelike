using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardDebuffSO : ScriptableObject
{
    public CardDebuff DebuffType => _type;
    [SerializeField] private CardDebuff _type;
    [TextArea] public string description;

    private Dictionary<MinorArcana, Action<MinorArcana>> PlayDic = new();
    private Dictionary<MinorArcana, Action<MinorArcana>> DrawDic = new();
    private Dictionary<MinorArcana, Action<MinorArcana>> DiscardDic = new();

    /// <summary>
    /// 디버프가 카드에 걸릴 때(Setup 직후) 한 번 호출, 카드를 뽑을때 한 번 호출
    /// </summary>
    public virtual void OnSubscribe(MinorArcana card, CardController controller)
    {
        Action<MinorArcana> play = c =>
        {
            if (c == card)
                OnCardPlayed(c, controller);
        };
        Action<MinorArcana> draw = c =>
        {
            if (c == card)
                OnSubscribe(c, controller);
        };
        Action<MinorArcana> discard = c =>
        {
            if (c == card)
                OnUnSubscribe(c, controller);
        };

        PlayDic[card] = play;
        controller.OnCardSubmited += play;

        DrawDic[card] = draw;
        controller.OnCardDrawn += draw;

        PlayDic[card] = discard;
        controller.OnCardDiscarded += discard;

    }
    /// <summary>
    /// 디버프 중인 카드가 패에서 벗어날 때 호출
    /// </summary>

    public virtual void OnUnSubscribe(MinorArcana card, CardController controller)
    {
        if (PlayDic.TryGetValue(card, out var play))
        {
            controller.OnCardSubmited -= play;
        }
        if (DrawDic.TryGetValue(card, out var draw))
        {
            controller.OnCardDrawn -= draw;
        }
        if (DiscardDic.TryGetValue(card, out var discard))
        {
            controller.OnCardSubmited -= discard;
        }
    }
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