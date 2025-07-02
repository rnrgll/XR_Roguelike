
using CustomUtility.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

// 마이너 아르카나를 보관하는 카드덱 클래스이다.
// 덱을 통해 카드의 인챈트 기능이나 카드의 추가 등을 다룬다.
public class CardDeck
{
    // 카드들을 직접 보관하는 딕셔너리 Deck이다.
    public Dictionary<MinorSuit, List<MinorArcana>> Deck;
    // 덱에 있는 카드들이 어떤 인챈트를 가지고 있는지 보관한다.
    public Dictionary<MinorArcana, CardEnchant> EnchantDic;
    public Dictionary<MinorArcana, CardDebuff> DebuffDic;
    // 아르카나의 최대 숫자를 정해준다.
    // 현재는 1~13까지의 카드와 시종 카드로 14장으로 이루어져 있다.
    [SerializeField] readonly int arcanaLength = 14;
    private readonly CsvTable _csvTable;
    public Action<MinorArcana, CardEnchant> OnCardEnchanted;
    public Action<MinorArcana, CardDebuff> OnCardDebuffed;
    // 카드가 추가될 때의 이벤트 (미사용)
    // public Action<MinorArcana> OnCardAdded;
    // 카드가 삭제될 때의 이벤트 (미사용)
    // public Action<MinorArcana> OnCardRemoved;

    public CardDeck(CsvTable csvTable)
    {
        _csvTable = csvTable;
        EnchantDic = new Dictionary<MinorArcana, CardEnchant>();
        DebuffDic = new Dictionary<MinorArcana, CardDebuff>();
        DeckInit();
    }

    private void DeckInit()
    {
        Deck = new Dictionary<MinorSuit, List<MinorArcana>>();
        foreach (MinorSuit _suit in Enum.GetValues(typeof(MinorSuit)))
        {
            Deck[_suit] = new List<MinorArcana>();
        }

        for (int suit = 0; suit < 4; suit++)
        {
            for (int num = 0; num < arcanaLength; num++)
            {
                Deck[(MinorSuit)suit].Add(GetCardData(suit, num));
            }
            Debug.Log((MinorSuit)suit);
        }
    }

    public MinorArcana GetCardData(int _suitIdx, int _num)
    {
        string[] data = _csvTable.GetLine(_suitIdx * arcanaLength + _num);
        string name = data[1];
        var suit = (MinorSuit)Enum.Parse(typeof(MinorSuit), data[4]);
        int numVal = int.Parse(data[5]);
        return new MinorArcana(name, suit, numVal);
    }

    public List<MinorArcana> GetAllCards()
    {
        var list = new List<MinorArcana>();
        for (int suit = 0; suit < 4; suit++)
        {
            for (int num = 0; num < arcanaLength; num++)
            {
                list.Add(Deck[(MinorSuit)suit][num]);
            }
        }
        return list;
    }

    #region Add/RemoveCard 
    /*
    public void AddCard(int _cardNum) => AddCard((MinorSuit)(_cardNum / arcanaLength), _cardNum % arcanaLength);
    /// <summary>
    /// 해당 카드를 덱에 추가해준다.
    /// </summary>
    public void AddCard(MinorSuit _suit, int _cardNum)
    {
        Deck[_suit]++;
        OnCardAdded?.Invoke(GetCardData((int)_suit, _cardNum));
    }

    public void RemoveCard(int _cardNum, int _order) => RemoveCard((MinorSuit)(_cardNum / arcanaLength), _cardNum % arcanaLength, _order);
    /// <summary>
    /// 해당 카드가 덱에 있는지 확인하고 있다면 한장 제거한다.
    /// </summary>
    public void RemoveCard(MinorSuit _suit, int _cardNum, int _order)
    {
        if (numOfCard[(int)_suit, _cardNum] == 0) return;
        numOfCard[(int)_suit, _cardNum]--;

        if (EnchantDic.ContainsKey(Deck[(int)_suit, _cardNum]))
        {
            EnchantDic.Remove(Deck[(int)_suit, _cardNum]);
        }
        OnCardRemoved?.Invoke(GetCardData((int)_suit, _cardNum));
    }*/
    #endregion

    public List<MinorArcana> GetEnchantableCard()
    {
        List<MinorArcana> EnchantedCardList = GetEnchantedCard();
        List<MinorArcana> EnchantableCard = new();
        for (int suit = 0; suit < 4; suit++)
        {
            for (int num = 0; num < 14; num++)
            {
                if (EnchantedCardList.Contains(Deck[(MinorSuit)suit][num])) continue;
                EnchantableCard.Add(Deck[(MinorSuit)suit][num]);
            }
        }
        return EnchantableCard;
    }

    private List<int> GetEnchantedCardNum()
    {
        ICollection<MinorArcana> keys = EnchantDic.Keys;
        List<int> keynum = new();
        foreach (var key in keys)
        {
            keynum.Add((int)key.CardSuit * 14 + key.CardNum);
        }
        return keynum;
    }

    public List<MinorArcana> GetEnchantedCard()
    {
        List<int> keyNum = GetEnchantedCardNum();
        List<MinorArcana> EnchantedCardList = new();
        foreach (int _num in keyNum)
        {
            EnchantedCardList.Add(Deck[(MinorSuit)(_num / 14)][_num % 14]);
        }
        return EnchantedCardList;
    }

    public void Enchant(MinorArcana _card, CardEnchant _enchant)
    {
        // List<MinorArcana> Enchantable = GetEnchantableCard();

        // if (!Enchantable.Contains(_card))
        // {
        //     throw new ArgumentException($"카드({_card})가 이미 강화되어 있습니다.");
        // }
        EnchantDic[_card] = _enchant;
        _card.Enchant.EnchantToCard(_enchant);
        OnCardEnchanted?.Invoke(_card, _enchant);
    }

    public void Debuff(MinorArcana _card, CardDebuff _debuff)
    {
        DebuffDic[_card] = _debuff;
        _card.debuff.DebuffToCard(_debuff);
        OnCardDebuffed?.Invoke(_card, _debuff);
    }

    public void DebuffClear(MinorArcana _card)
    {
        DebuffDic.Remove(_card);
        _card.debuff.DebuffToCard(CardDebuff.none);
    }
}

