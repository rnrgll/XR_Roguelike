
using CustomUtility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    public MinorArcana[,] Deck;
    public int[,] numOfCard { private set; get; }
    public Dictionary<(MinorArcana card, int order), CardEnchant> EnchantDic;
    [SerializeField] int arcanaLength = 14;
    public Action<MinorArcana> OnCardAdded;
    public Action<MinorArcana> OnCardRemoved;
    private string ArcanaTableLink =
    "https://docs.google.com/spreadsheets/d/1epFe-PfQ0mA9D7wl7_ByiZhfj-A5pPPO/export?format=csv&range=A4:I59&ouid=106241391175336675849&rtpof=true&sd=true";
    private CsvTable filePath;

    public CardDeck()
    {
        CsvLoad();
        Init();
    }

    private void CsvLoad()
    {
        CardCsvDownLoader.Start(ArcanaTableLink, "ArcanaCsv");
    }

    private void Init()
    {
        Deck = new MinorArcana[4, 14];
        numOfCard = new int[4, 14];
        EnchantDic = new Dictionary<(MinorArcana card, int order), CardEnchant>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 14; j++)
            {
                numOfCard[i, j] = 1;
                Deck[i, j] = GetCardData((MinorSuit)i, j);
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
        numOfCard[(int)_suit, _cardNum]++;
        OnCardAdded?.Invoke(GetCardData(_suit, _cardNum));
    }

    public void RemoveCard(int _cardNum, int _order) => RemoveCard((MinorSuit)(_cardNum / arcanaLength), _cardNum % arcanaLength, _order);
    /// <summary>
    /// 해당 카드가 덱에 있는지 확인하고 있다면 한장 제거한다.
    /// </summary>
    public void RemoveCard(MinorSuit _suit, int _cardNum, int _order)
    {
        if (numOfCard[(int)_suit, _cardNum] == 0) return;
        numOfCard[(int)_suit, _cardNum]--;

        if (EnchantDic.ContainsKey((Deck[(int)_suit, _cardNum], _order)))
        {
            EnchantDic.Remove((Deck[(int)_suit, _cardNum], _order));
        }
        OnCardRemoved?.Invoke(GetCardData(_suit, _cardNum));
    }
    #endregion

    public void Enchant(MinorSuit _suit, int _cardNum, int _order)
    {
        CardEnchant nowEnchant = EnchantDic[(Deck[(int)_suit, _cardNum], _order)];

        if (nowEnchant != CardEnchant.Maximum)
        {
            EnchantDic[(Deck[(int)_suit, _cardNum], _order)] = (CardEnchant)((int)nowEnchant + 1);
        }
    }

    public MinorArcana GetCardData(MinorSuit _suit, int _cardNum)
    {
        string[] cardData = filePath.GetLine((int)_suit * 14 + _cardNum);
        return new MinorArcana(cardData[0], (MinorSuit)Enum.Parse(typeof(MinorSuit), cardData[4]), int.Parse(cardData[5]));
    }

}

