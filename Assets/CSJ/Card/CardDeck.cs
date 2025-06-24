
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public MinorArcana[,] Deck;
    public int[,] numOfCard { private set; get; }
    [SerializeField] int arcanaLength = 14;
    public Action<MinorArcana> OnCardAdded;
    public Action<MinorArcana> OnCardRemoved;

    public CardDeck()
    {
        Init();
    }

    private void Init()
    {
        Deck = new MinorArcana[4, 14];
        numOfCard = new int[4, 14];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                numOfCard[i, j] = 1;
                Deck[i, j] = new MinorArcana((MinorSuit)i, j + 1);
            }
        }
    }

    #region Add/RemoveCard 
    public void AddCard(int _cardNum) => AddCard((MinorSuit)(_cardNum / arcanaLength), _cardNum % arcanaLength);
    /// <summary>
    /// 해당 카드를 덱에 추가해준다.
    /// </summary>
    public void AddCard(MinorSuit _suit, int _cardNum)
    {
        int _order = numOfCard[(int)_suit, _cardNum]++;
        OnCardAdded?.Invoke(new MinorArcana(_suit, _cardNum));
    }

    public void RemoveCard(int _cardNum, int _order) => RemoveCard((MinorSuit)(_cardNum / arcanaLength), _cardNum % arcanaLength, _order);
    /// <summary>
    /// 해당 카드가 덱에 있는지 확인하고 있다면 한장 제거한다.
    /// </summary>
    public void RemoveCard(MinorSuit _suit, int _cardNum, int _order)
    {
        if (numOfCard[(int)_suit, _cardNum] == 0) return;
        numOfCard[(int)_suit, _cardNum]--;

        OnCardRemoved?.Invoke(new MinorArcana(_suit, _cardNum));
    }
    #endregion

}

