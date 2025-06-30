using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using manager = Managers.Manager;
using CardEnum;
using CustomUtility.IO;

/// <summary>
/// 카드를 컨트롤하는 메서드
/// 시작할 때 CardDeck을 생성한다.
/// </summary>
public class CardController : MonoBehaviour
{
    // 마이너 아르카나들의 정보를 받아오는 링크로 구글스프레드 시트와 연결되어 있다.
    private readonly string ArcanaTableLink =
    "https://docs.google.com/spreadsheets/d/1epFe-PfQ0mA9D7wl7_ByiZhfj-A5pPPO/export?format=csv&range=A4:I59&ouid=106241391175336675849&rtpof=true&sd=true";
    // 위에서 받아온 정보가 저장된 경로이다.
    private CsvTable _csvTable;
    public DeckPile<MinorArcana> DeckPile;
    public HandPile<MinorArcana> Hand;
    public GraveyardPile<MinorArcana> Graveyard;
    public BattlePile<MinorArcana> BattleDeck;
    public List<int> _selectedNums;
    public int sumofNums;
    private List<MinorArcana> disposableCardList;
    private List<MinorArcana> SelectedCard;
    private CardCombinationEnum cardComb;

    public CardDeck Deck;

    public int drawNum = 8;
    public Action OnChangedHands = delegate { };
    public Action<MinorArcana> OnCardSelected = delegate { };
    public Action<MinorArcana> OnCardDeSelected = delegate { };
    public Action<CardCombinationEnum> OnSelectionChanged;

    // public BattleStat stat; TODO: 플레이어 스탯과 연계

    #region 미사용 코드
    // public Dictionary<CardStatus, List<MinorArcana>> CardListDic;

    // public List<MinorArcana> Hand;
    #endregion



    private void Start()
    {

        StartCoroutine(
            CardCsvDownLoader.Start(
                ArcanaTableLink,
                table =>
                {
                    _csvTable = table;
                    Deck = new CardDeck(_csvTable);
                    Init();

                    OnChangedHands.Invoke();
                }
            )
        );
    }

    public void OnEnable()
    {
        OnCardSelected += AddSelect;
        OnCardDeSelected += RemoveSelect;
    }

    public void OnDisable()
    {
        OnCardSelected -= AddSelect;
        OnCardDeSelected -= RemoveSelect;
    }

    public void Init()
    {
        Debug.Log("init");
        DeckPile = new DeckPile<MinorArcana>();
        Hand = new HandPile<MinorArcana>();
        Graveyard = new GraveyardPile<MinorArcana>();
        BattleDeck = new BattlePile<MinorArcana>();
        SelectedCard = new();
        ResetDeck();
        disposableCardList = new();
        BattleInit();



        #region 미사용 코드
        // CardDicInit();
        // Hand = CardListDic[CardStatus.Hand];
        // stat = new BattleStat();
        // stat.Init();
        // Deck.OnCardAdded += OnCardAddedDeck;
        // Deck.OnCardRemoved += OnCardRemoveDeck;
        #endregion
    }

    public void ResetDeck()
    {
        foreach (MinorArcana card in Deck.GetAllCards())
        {
            DeckPile.Add(card);
        }
    }

    public void BattleInit()
    {
        Debug.Log("Battle");
        ClearDeck();
        foreach (var card in DeckPile.Cards)
        {
            BattleDeck.Add(card);
        }
        BattleDeck.Shuffle();
        Draw(8);
    }

    public void ClearDeck()
    {
        Hand.Clear();
        BattleDeck.Clear();
        Graveyard.Clear();
    }

    public void Draw() => Draw(drawNum - Hand.Count);


    // TODO : 추후 만약 드로우와 관계된 스탯이 생긴다면 해당 값을 지정해주어야 한다.
    // 현재는 8장 고정이므로 Draw를 8장 뽑는다.
    public void Draw(int n)
    {
        if (BattleDeck.Count == 0) DeckRefill();
        for (int i = 0; i < n; i++)
        {
            var card = BattleDeck.Draw();
            Hand.Add(card);
            if (BattleDeck.Count != 0) continue;
            DeckRefill();
            if (BattleDeck.Count == 0)
            {
                Debug.Log("덱과 묘지가 모두 0장입니다.");
                break;
            }
        }
        OnChangedHands?.Invoke();
    }

    public void DeckRefill()
    {
        foreach (var card in Graveyard.Cards)
        {
            BattleDeck.Add(card);
        }
        Graveyard.Clear();
    }

    public int Discard(List<MinorArcana> cards)
    {
        int _num = 0;
        foreach (MinorArcana _card in cards)
        {
            if (!Hand.GetCardList().Contains(_card)) continue;

            if (disposableCardList.Remove(_card))
            {
                Hand.Remove(_card);
                _num++;
                continue;
            }
            Graveyard.Add(_card);
            Hand.Remove(_card);
            _num++;
        }
        OnChangedHands?.Invoke();
        return _num;
    }

    public void SwapCard(MinorArcana deckCard, MinorArcana HandCard)
    {
        BattleDeck.Swap(deckCard, HandCard);
        Hand.Swap(HandCard, deckCard);
        OnChangedHands?.Invoke();
    }

    public List<MinorArcana> GetHand()
    {
        IEnumerable<MinorArcana> MinorArcanas = Hand.Cards;
        List<MinorArcana> cards = new();
        foreach (MinorArcana card in MinorArcanas)
        {
            cards.Add(card);
        }
        return cards;
    }

    public void SortBySuit()
    {
        var temp = new List<MinorArcana>();
        foreach (MinorArcana card in Hand.Cards)
        {
            temp.Add(card);
        }

        temp.Sort((a, b) =>
        {
            int result = a.CardSuit.CompareTo(b.CardSuit);
            if (result != 0)
                return result;
            return a.CardNum.CompareTo(b.CardNum);
        });

        Hand.Clear();
        foreach (MinorArcana card in temp)
        {
            Hand.Add(card);
        }
        OnChangedHands?.Invoke();
    }

    public void SortByNum()
    {
        var temp = new List<MinorArcana>();
        foreach (MinorArcana card in Hand.Cards)
        {
            temp.Add(card);
        }

        temp.Sort((a, b) =>
        {
            int result = a.CardNum.CompareTo(b.CardNum);
            if (result != 0)
                return result;
            return a.CardSuit.CompareTo(b.CardSuit);
        });

        Hand.Clear();
        foreach (MinorArcana card in temp)
        {
            Hand.Add(card);
        }
        OnChangedHands?.Invoke();
    }

    public void AddDisposableCard(MinorArcana _disposCard)
    {
        Hand.Add(_disposCard);
        disposableCardList.Add(_disposCard);
        OnChangedHands?.Invoke();
    }

    public void AddSelect(MinorArcana card)
    {
        SelectedCard.Add(card);
        CardCombCal();
    }

    public void RemoveSelect(MinorArcana card)
    {
        SelectedCard.Remove(card);
        CardCombCal();
    }

    public void CardCombCal()
    {
        cardComb = CardCombination.CalCombination(SelectedCard, out _selectedNums);
        sumofNums = 0;
        foreach (int i in _selectedNums)
        {
            if (i == 1)
            {
                sumofNums += 10;
            }
            else sumofNums += i;
        }
        OnSelectionChanged?.Invoke(cardComb);
    }


    public List<MinorArcana> GetSelection()
    {
        return SelectedCard;
    }

    public int GetSumOfNums()
    {
        return sumofNums;
    }





    #region 미사용 코드(딕셔너리 관련)
    /*
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
            foreach (MinorSuit _suit in Enum.GetValues(typeof(MinorSuit)))
            {
                foreach (MinorArcana card in Deck.Deck[_suit])
                {
                    CardListDic[CardStatus.DeckList].Add(card);
                }
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

            CardListDic[CardStatus.Graveyard].Clear();

            Shuffle(CardListDic[CardStatus.BattleDeck]);
        }
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

    public void SwapCards(MinorArcana deckCard, MinorArcana handCard)
    {
        var battleDeck = CardListDic[CardStatus.BattleDeck];
        List<MinorArcana> tempCards = new();
        battleDeck.Remove(handCard);
        Hand[Hand.IndexOf(handCard)] = deckCard;
        Shuffle(battleDeck);
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
            if (disposableCardList.Remove(_card))
            {
                CardListDic[CardStatus.Hand].Remove(_card);
                _num++;
                continue;
            }
            CardListDic[CardStatus.Graveyard].Add(_card);

            CardListDic[CardStatus.Hand].Remove(_card);
            _num++;
        }
        return _num;
    }
        */
    #endregion

    #region 미사용 코드(덱 추가 이벤트)
    /*
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
    */
    #endregion

    #region 단순 미사용
    /*    /// <summary>
    /// Fisher-Yates Shuffle 알고리즘을 이용한 셔플
    /// </summary>
    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = manager.randomManager.RandInt(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }*/
    #endregion




}