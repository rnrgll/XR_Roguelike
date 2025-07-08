using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;
using System;

[CreateAssetMenu(fileName = "new DisposableCard", menuName = "Cards/DisposableCard")]
public class DisposableCardSO : ScriptableObject
{
    [Header("카드 정보")]
    public string cardName;
    public Sprite sprite;
    public DisposableCardName DisposableCard;

    [Header("MinorArcana 정보")]
    public MinorSuit suit = MinorSuit.Special;
    public int cardNum = 0;


    protected MinorArcana disposableCard;
    protected CardController controller;
    protected PlayerController playerController;
    protected Dictionary<MinorArcana, Action<MinorArcana>> PlayDic = new();
    protected Dictionary<MinorArcana, Action<MinorArcana>> DrawDic = new();
    protected Dictionary<MinorArcana, Action<MinorArcana>> DiscardDic = new();


    public void InitializeSO(PlayerController _playerController)
    {
        playerController = _playerController;
        controller = playerController.GetCardController();
    }

    public void AddDisposableCard()
    {
        controller.ApplayDisposableCard(DisposableCard);
    }

    public MinorArcana CreateCard()
    {
        return disposableCard = new MinorArcana(cardName, suit, cardNum);
    }

    public virtual void OnCardPlayed(MinorArcana card) { }

    public virtual void OnSubscribe(MinorArcana card)
    {
        Action<MinorArcana> play = c =>
        {
            if (c == card)
                OnCardPlayed(c);
        };
        Action<MinorArcana> discard = c =>
        {
            if (c == card)
                OnUnSubscribe(c);
        };
        PlayDic[card] = play;
        controller.OnCardSubmited += play;

        PlayDic[card] = discard;
        controller.OnCardDiscarded += discard;

    }

    public virtual void OnUnSubscribe(MinorArcana card)
    {
        if (PlayDic.TryGetValue(card, out var play))
        {
            playerController.GetCardController().OnCardSubmited -= play;
        }
        if (DiscardDic.TryGetValue(card, out var discard))
        {
            playerController.GetCardController().OnCardSubmited -= discard;
        }
    }
    public void RemoveCard()
    {
        playerController.GetCardController().RemoveDisposableCard(disposableCard);
    }

    public MinorArcana GetDisposCard()
    {
        return disposableCard;
    }


}
