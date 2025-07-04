using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "new DisposableCard", menuName = "Cards/DisposableCard")]
public class DisposableCardSO : ScriptableObject
{
    [Header("카드 정보")]
    public string cardName;
    public Sprite sprite;

    [Header("MinorArcana 정보")]
    public MinorSuit suit = MinorSuit.Special;
    public int cardNum = 0;

    [Header("부가 효과")]
    public DisposableCardSO disposableCard;

    public void AddDisposableCard()
    {
        TurnManager.Instance.GetPlayerController().GetCardController().
        AddDisposableCard(this, new MinorArcana(cardName, suit, cardNum));
    }

    public void OnCardPlayed()
    {

    }

}
