using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using manager = Managers.Manager;

/// <summary>
/// 카드를 컨트롤하는 메서드
/// 시작할 때 CardDeck을 생성하고 일단 카드의 추가와 삭제와 같은 부분도 처리가 되어있다.
/// </summary>
public class CardController : MonoBehaviour
{
    public CardDeck Deck;
    public List<MinorArcana> Hand;
    public Dictionary<CardStatus, List<MinorArcana>> CardListDic;
    public int drawNum = 8;

    // public BattleStat stat; TODO: 플레이어 스탯과 연계

    private void Awake() => Init();

    public void Init()
    {
        Deck = new CardDeck();
        CardDicInit();
        ResetDeck();
        // stat = new BattleStat();
        // stat.Init();
        Hand = CardListDic[CardStatus.Hand];

        Deck.OnCardAdded += OnCardAddedDeck;
        Deck.OnCardRemoved += OnCardRemoveDeck;
    }

    /// <summary>
    /// 카드 딕셔너리를 만든다.
    /// CardStatus의 값을 하나씩 받아와서 각 상태별로 딕셔너리에 등록한다.
    /// </summary>
    private void CardDicInit()
    {
        CardListDic = new Dictionary<CardStatus, List<MinorArcana>>();
        foreach (CardStatus _status in Enum.GetValues(typeof(CardStatus)))
        {
            CardListDic[_status] = new List<MinorArcana>();
        }
    }

    /// <summary>
    /// 카드 덱에서 카드를 하나씩 가져와 덱 리스트에 추가한다.
    /// </summary>
    public void ResetDeck()
    {
        foreach (MinorArcana card in Deck.Deck)
        {
            CardListDic[CardStatus.DeckList].Add(card);
        }
    }


    // TODO : 혹시 덱에 카드가 추가될 수도 있기에 이벤트를 일단 제작해두었습니다.
    /// <summary>
    /// 덱에서 카드가 추가되었을 때 이벤트로 받아 현재 덱에 추가한다.
    /// 카드의 문양을 먼저 비교하고 그 후 숫자를 비교한다.
    /// 문양은 enum의 Wands, Cups, Swords, Pentacles 순서대로 정렬된다.
    /// </summary>
    private void OnCardAddedDeck(MinorArcana _card)
    {
        CardListDic[CardStatus.DeckList].Add(_card);
        SortBySuit(CardStatus.DeckList);
    }

    /// <summary>
    /// 덱에서 카드가 삭제되었을 때 이벤트로 받아서 현재 덱에서 삭제한다.
    /// </summary>
    private void OnCardRemoveDeck(MinorArcana _card)
    {
        CardListDic[CardStatus.DeckList].Remove(_card);
    }

    /// <summary>
    /// 덱에서 핸드로 카드를 뽑아오고 배틀 덱에서 제거한다.
    /// </summary>

    // TODO : 추후 만약 드로우와 관계된 스탯이 생긴다면 해당 값을 지정해주어야 한다.
    // 현재는 8장 고정이므로 Draw를 8장 뽑는다.
    public void Draw() => Draw(drawNum - Hand.Count);

    public void Draw(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            Hand.Add(CardListDic[CardStatus.BattleDeck][0]);

            CardListDic[CardStatus.BattleDeck].RemoveAt(0);
            if (CardListDic[CardStatus.BattleDeck].Count != 0) continue;
            DeckRefill();
            if (CardListDic[CardStatus.BattleDeck].Count == 0) break;
        }
    }

    public void SortBySuit(CardStatus _status)
    {
        CardListDic[_status].Sort((a, b) =>
        {
            int result = a.CardSuit.CompareTo(b.CardSuit);
            if (result != 0) return result;
            return a.CardNum.CompareTo(b.CardNum);
        });
    }

    public void SortByNum(CardStatus _status)
    {
        CardListDic[_status].Sort((a, b) =>
        {
            int result = a.CardNum.CompareTo(b.CardNum);
            if (result != 0) return result;
            return a.CardSuit.CompareTo(b.CardSuit);
        });
    }

    public List<MinorArcana> GetHand()
    {
        return Hand;
    }


    /// <summary>
    /// 카드 버리기, 8장이 넘었을 때 카드를 선택하여 버린다.
    /// 버릴 카드를 list로 받아서 무덤에 해당 카드를 추가하고
    /// Hand에서 해당 카드를 삭제한다. 
    /// </summary>
    /// <param name="cards"> 버릴 카드의 리스트 </param>
    /// <returns>총 버린 카드의 갯수를 반환한다.</returns>
    public int Discard(List<MinorArcana> cards)
    {
        int _num = 0;
        foreach (MinorArcana _card in cards)
        {
            CardListDic[CardStatus.Graveyard] =
            CardListDic[CardStatus.Hand].ToList();

            CardListDic[CardStatus.Hand].Remove(_card);
            _num++;
        }
        return _num;
    }

    /// <summary>
    /// Fisher-Yates Shuffle 알고리즘을 이용한 셔플
    /// </summary>
    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;

        for (int i = n - 1; i > 0; i--)
        {
            // int j = UnityEngine.Random.Range(0, i + 1);
            int j = manager.randomManager.RandInt(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public void BattleInit()
    {
        CardListDic[CardStatus.BattleDeck] =
        CardListDic[CardStatus.DeckList].ToList();

        Shuffle(CardListDic[CardStatus.BattleDeck]);
    }

    public void DeckRefill()
    {
        CardListDic[CardStatus.BattleDeck] =
        CardListDic[CardStatus.Graveyard].ToList();

        Shuffle(CardListDic[CardStatus.BattleDeck]);
    }
}