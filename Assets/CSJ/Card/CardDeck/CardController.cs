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
    #region 미사용 코드
    // public BattleStat stat; TODO: 플레이어 스탯과 연계


    // public Dictionary<CardStatus, List<MinorArcana>> CardListDic;

    // public List<MinorArcana> Hand;
    #endregion

    #region csv 관련 모음
    // 마이너 아르카나들의 정보를 받아오는 링크로 구글스프레드 시트와 연결되어 있다.
    private readonly string ArcanaTableLink =
    "https://docs.google.com/spreadsheets/d/1epFe-PfQ0mA9D7wl7_ByiZhfj-A5pPPO/export?format=csv&range=A4:I59&ouid=106241391175336675849&rtpof=true&sd=true";
    // 위에서 받아온 정보가 저장된 경로이다.
    private CsvTable _csvTable;
    #endregion

    #region 각종 외부 참조에 사용되는 모음
    public List<int> _selectedNums;
    public CardDeck Deck { get; private set; }
    public int sumofNums { get; private set; }
    public Dictionary<CardBonus, List<float>> TurnBonusDic;
    public Dictionary<CardBonus, List<float>> BattleBonusDic;
    public Dictionary<CardBonus, List<float>> TurnPenaltyDic;
    public Dictionary<CardBonus, List<float>> BattlePenaltyDic;
    public List<MinorArcana> SelectedCard { get; private set; } = new();
    public CardCombinationEnum cardComb { get; private set; }
    public int[] numbersList = new int[15];
    public int[] SuitsList = new int[4];
    public Dictionary<CardCombinationEnum, float> ComboMultDic;
    #endregion

    #region  설정 모음 
    public int drawNum = 8;
    public int DiscardCount;
    public int DiscardSetting = 3;
    public Dictionary<MinorArcana, bool> IsUsableDic = new();
    public Dictionary<MinorArcana, int> MultiPleCardDic = new();
    public CardSortEnum sortStand = CardSortEnum.Suit;
    private CardController cardController;
    public bool IsReady { get; private set; } = false;

    #endregion

    #region  카드 상태 선언 모음
    public DeckPile<MinorArcana> DeckPile { get; private set; }
    public HandPile<MinorArcana> Hand { get; private set; }
    public GraveyardPile<MinorArcana> Graveyard { get; private set; }
    public BattlePile<MinorArcana> BattleDeck { get; private set; }
    #endregion



    #region 이벤트 함수들 선언 모음
    public Action OnChangedHands = delegate { };
    public Action<MinorArcana> OnCardSelected = delegate { };
    public Action<MinorArcana> OnCardDeSelected = delegate { };
    public Action<CardCombinationEnum> OnSelectionChanged;
    public Action<List<MinorArcana>> OnSubmit;
    public Action<MinorArcana> OnCardDrawn;
    public Action<MinorArcana> OnCardDiscarded;
    public Action<MinorArcana> OnCardSubmited;
    public Action<MinorArcana, MinorArcana> OnCardSwapped;
    private Action<MinorArcana, CardEnchant> _onEnchantApplied;
    private Action<MinorArcana, CardEnchant> _onEnchantCleared;
    private Action<MinorArcana, CardDebuff> _onDebuffApplied;
    private Action<MinorArcana, CardDebuff> _onDebuffCleared;
    private Action<MinorArcana> OnRemoveStatusEffectCard;
    public event Action<int> OnEnterSelectionMode;
    public event Action OnExitSelectionMode;
    public Action<CardController> OnInitCardController;
    public Action OnReady;
    #endregion

    #region SO관련 내용 모음
    [SerializeField] private CardEnchantSO[] EnchantArr;
    [SerializeField] private CardDebuffSO[] DebuffArr;
    public Dictionary<CardDebuff, CardDebuffSO> DebuffDic { get; private set; } = new();
    [SerializeField] private StatusEffectCardSO[] StatusEffectArr;
    public Dictionary<StatusEffect, StatusEffectCardSO> StatusEffectDic { get; private set; } = new();
    public Dictionary<MinorArcana, StatusEffectCardSO> statusEffectCardDic { get; private set; } = new();
    [SerializeField] private DisposableCardSO[] DisposableArr;
    public Dictionary<DisposableCardName, DisposableCardSO> disposableDic { get; private set; } = new();
    public Dictionary<MinorArcana, DisposableCardSO> disposableCardDic { get; private set; } = new();
    [SerializeField] public MajorArcanaSO[] majorArcanaArr;
    #endregion



    #region 각종 초기화용 함수들
    private IEnumerator Start()
    {
        Debug.Log("[CC] Start 함수 호출");
        yield return CsvLoadCoroutine();
        OnChangedHands.Invoke();
        OnReady?.Invoke();
        IsReady = true;
        Debug.Log("[cc]Start 함수 종료");
    }

    private IEnumerator CsvLoadCoroutine()
    {
        yield return CardCsvDownLoader.Start(
            ArcanaTableLink, table =>
            {
                _csvTable = table;
                Deck = new CardDeck(_csvTable);
                Init();
            }
        );
    }

    public void Init()
    {
        DeckPile = new DeckPile<MinorArcana>();
        Hand = new HandPile<MinorArcana>();
        Graveyard = new GraveyardPile<MinorArcana>();
        BattleDeck = new BattlePile<MinorArcana>();

        foreach (var debuffSO in DebuffArr)
        {
            DebuffDic[debuffSO.DebuffType] = debuffSO;
        }
        foreach (var StatusEffectSO in StatusEffectArr)
        {
            StatusEffectDic[StatusEffectSO.statusEffect] = StatusEffectSO;
        }
        foreach (var DisposableCardSO in DisposableArr)
        {
            disposableDic[DisposableCardSO.DisposableCard] = DisposableCardSO;
        }


        BattleBonusDic = new();
        BattlePenaltyDic = new();
        TurnPenaltyDic = new();
        TurnBonusDic = new();

        foreach (CardBonus type in Enum.GetValues(typeof(CardBonus)))
        {
            BattleBonusDic[type] = new();
            BattlePenaltyDic[type] = new();
            TurnPenaltyDic[type] = new();
            TurnBonusDic[type] = new();
        }



        ComboMultDic = new Dictionary<CardCombinationEnum, float>()
        {
            {CardCombinationEnum.HighCard , 1 },
            {CardCombinationEnum.OnePair ,2 },
            {CardCombinationEnum.TwoPair ,3 },
            {CardCombinationEnum.Triple, 4},
            {CardCombinationEnum.Straight, 6},
            {CardCombinationEnum.Flush, 8},
            {CardCombinationEnum.FullHouse, 12},
            {CardCombinationEnum.FourCard, 20},
            {CardCombinationEnum.StraightFlush, 30},
            {CardCombinationEnum.FiveCard, 50},
            {CardCombinationEnum.FiveJoker, 1},
        };

        ResetDeck();

        #region 미사용 코드
        // CardDicInit();
        // Hand = CardListDic[CardStatus.Hand];
        // stat = new BattleStat();
        // stat.Init();
        // Deck.OnCardAdded += OnCardAddedDeck;
        // Deck.OnCardRemoved += OnCardRemoveDeck;
        #endregion
    }

    public void InitializeCC(PlayerController playerController)
    {
        cardController = this;
        foreach (var so in EnchantArr)
            so.InitializeSO(playerController);
        foreach (var so in DebuffArr)
            so.InitializeSO(playerController);
        foreach (var so in DisposableArr)
            so.InitializeSO(playerController);
        foreach (var so in StatusEffectArr)
            so.InitializeSO(playerController);
        foreach (var so in majorArcanaArr)
            so.InitializeSO(playerController);
        SubscribeAll();
    }

    private void SubscribeAll()
    {
        OnCardSelected += AddSelect;
        OnCardDeSelected += RemoveSelect;
        OnCardDrawn += AdjustCountList;

        // “디버프가 새로 걸릴 때” SO 구독
        _onEnchantApplied = (card, debuff) =>
        {
            var so = Deck.GetEnchantSO(card);
            so?.OnSubscribe(card);
        };
        Deck.OnCardEnchanted += _onEnchantApplied;


        // “디버프가 해제될 때” SO 해제
        _onEnchantCleared = (card, oldEnchant) =>
        {
            var so = Deck.GetEnchantSO(card);
            so?.OnUnSubscribe(card);
        };
        Deck.OnCardEnchantCleared += _onEnchantCleared;

        // “디버프가 새로 걸릴 때” SO 구독
        _onDebuffApplied = (card, debuff) =>
        {
            var so = Deck.GetDebuffSO(card);
            so?.OnSubscribe(card);
        };
        Deck.OnCardDebuffed += _onDebuffApplied;


        // “디버프가 해제될 때” SO 해제
        _onDebuffCleared = (card, oldDebuff) =>
        {
            var so = Deck.GetDebuffSO(card);
            so?.OnUnSubscribe(card);
        };
        Deck.OnCardDebuffCleared += _onDebuffCleared;
    }

    public void BattleInit()
    {
        // TurnManager.Instance.GetPlayerController().OnTurnStarted += TurnInit;
        ClearDeck();
        foreach (var card in DeckPile.Cards)
        {
            BattleDeck.Add(card);
        }
        numbersList[0] = 0;
        for (int i = 1; i < 15; i++)
        {
            numbersList[i] = 0;
        }
        for (int j = 0; j < 4; j++)
        {
            SuitsList[j] = 0;
        }

        foreach (var i in BattleDeck.GetCardList())
        {
            SuitsList[(int)i.CardSuit]++;
            numbersList[i.CardNum]++;
        }
        foreach (var kv in TurnBonusDic)
            kv.Value.Clear();
        foreach (var kv in BattleBonusDic)
            kv.Value.Clear();
        foreach (var kv in TurnPenaltyDic)
            kv.Value.Clear();
        foreach (var kv in BattlePenaltyDic)
            kv.Value.Clear();


        DiscardCount = DiscardSetting;
        BattleDeck.Shuffle();
        Draw();
    }

    // TODO: 추후 계약카드 드로우 감소 적용
    public void TurnInit()
    {
        foreach (var kv in TurnBonusDic)
            kv.Value.Clear();
        foreach (var kv in TurnPenaltyDic)
            kv.Value.Clear();
        Draw();
    }
    #endregion

    #region 덱 관련 함수들
    public void ResetDeck()
    {
        foreach (MinorArcana card in Deck.GetAllCards())
        {
            DeckPile.Add(card);
        }
    }

    public void ClearDeck()
    {
        if (Hand == null) return;
        Hand.Clear();
        BattleDeck.Clear();
        Graveyard.Clear();
    }

    #endregion

    #region 반환용 함수 모음
    public List<MinorArcana> GetSelection()
    {
        return SelectedCard;
    }

    public void Submit()
    {
        if (SelectedCard == null)
        {
            Debug.LogError("SelectedCard is null!");
            return;
        }
        var toPlay = SelectedCard.ToList();
        //penaltyAmount = 0;
        foreach (var card in SelectedCard)
        {
            OnCardSubmited?.Invoke(card);
        }
        OnSubmit?.Invoke(SelectedCard);

        Discard(toPlay);

        ClearSelect();
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
    public CardDebuffSO GetDebuffSO(CardDebuff _debuff)
    {
        return DebuffDic[_debuff];
    }
    public StatusEffectCardSO GetStatusEffectSO(StatusEffect _statusEffect)
    {
        return StatusEffectDic[_statusEffect];
    }

    public float GetCardBonus(CardBonus bonus)
    {
        float value = 0;
        if (TurnBonusDic.ContainsKey(bonus))
            foreach (var i in TurnBonusDic[bonus])
            {
                value += i;
            }
        if (BattleBonusDic.ContainsKey(bonus))
            foreach (var i in BattleBonusDic[bonus])
            {
                value += i;
            }
        if (TurnBonusDic.ContainsKey(bonus))
            foreach (var i in TurnPenaltyDic[bonus])
            {
                value -= i;
            }
        if (BattlePenaltyDic.ContainsKey(bonus))
            foreach (var i in BattlePenaltyDic[bonus])
            {
                value -= i;
            }
        return value;
    }

    #endregion

    #region 카드를 직접 컨트롤하는 함수 모음
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
            if (card.debuff.debuffinfo != CardDebuff.none)
                Deck.GetDebuffSO(card)?.OnSubscribe(card);

            if (card.Enchant.enchantInfo != CardEnchant.none)
                Deck.GetEnchantSO(card)?.OnSubscribe(card);

            OnCardDrawn?.Invoke(card);

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

    public void Draw(MinorArcana card)
    {
        List<MinorArcana> deckList = BattleDeck.GetCardList();
        if (!deckList.Contains(card)) return;
        BattleDeck.Remove(card);
        Hand.Add(card);
        if (card.debuff.debuffinfo != CardDebuff.none)
            Deck.GetDebuffSO(card)?.OnSubscribe(card);

        if (card.Enchant.enchantInfo != CardEnchant.none)
            Deck.GetEnchantSO(card)?.OnSubscribe(card);

        OnCardDrawn?.Invoke(card);

        if (BattleDeck.Count == 0)
        {
            DeckRefill();
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
        var cardsList = cards.ToList();
        foreach (MinorArcana _card in cardsList)
        {
            if (!Hand.GetCardList().Contains(_card)) continue;

            if (disposableCardDic.ContainsKey(_card))
            {
                RemoveDisposableCard(_card);
                _num++;
                continue;
            }
            Graveyard.Add(_card);
            Hand.Remove(_card);
            if (_card.debuff.debuffinfo != CardDebuff.none)
                Deck.GetDebuffSO(_card).OnUnSubscribe(_card);
            if (_card.Enchant.enchantInfo != CardEnchant.none)
                Deck.GetEnchantSO(_card)?.OnUnSubscribe(_card);

            OnCardDiscarded?.Invoke(_card);
            _num++;
        }
        ClearSelect();
        OnChangedHands?.Invoke();
        return _num;
    }

    public void SwapCard(MinorArcana deckCard, MinorArcana HandCard)
    {
        BattleDeck.Swap(deckCard, HandCard);
        Hand.Swap(HandCard, deckCard);

        AdjustCountList(deckCard);
        numbersList[HandCard.CardNum]++;
        SuitsList[(int)HandCard.CardSuit]++;

        OnCardSwapped(deckCard, HandCard);

        OnChangedHands?.Invoke();
    }

    public void SortBySuit()
    {
        ClearSelect();
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
    }

    public void SortByNum()
    {
        ClearSelect();
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
    }

    public void SortByStand()
    {
        if (sortStand == CardSortEnum.Number)
            SortByNum();
        else
            SortBySuit();
    }

    public void ChangeSortStand()
    {
        sortStand ^= CardSortEnum.Suit;
        OnChangedHands?.Invoke();
    }
    public void ChangeStandBySuit()
    {
        sortStand = CardSortEnum.Suit;
        OnChangedHands?.Invoke();
    }
    public void ChangeStandByNum()
    {
        sortStand = CardSortEnum.Number;
        OnChangedHands?.Invoke();
    }

    public void exchangeHand(List<MinorArcana> cards)
    {
        int beforeCount = Hand.Count;
        int discCount = Discard(cards);
        ClearSelect();
        int afterCount = Hand.Count;
        int targetCount = Mathf.Min(beforeCount, drawNum);


        int toDraw = targetCount - afterCount;
        if (toDraw > 0)
        {
            Draw(toDraw);
        }
        OnChangedHands?.Invoke();
    }

    public void AdjustCountList(MinorArcana card)
    {
        numbersList[card.CardNum]--;
        SuitsList[(int)card.CardSuit]--;
    }

    #endregion

    #region 임시 카드 추가 함수
    /// <summary>
    /// 일회성 카드를 패에 추가한다.
    /// 직접 호출하지 말고 DisposableCardSo에서 AddDisposCard로 호출
    /// </summary>
    /// <param name="_disposCard">
    /// Card의 Suit는 특수 카드의 경우 Special 이외에는 해당 문양으로 지정한다.
    /// </param>
    private void AddDisposableCard(DisposableCardSO disposableCardSO, MinorArcana _disposCard)
    {
        Hand.Add(_disposCard);
        disposableCardDic[_disposCard] = disposableCardSO;
        disposableCardSO.OnSubscribe(_disposCard);
        OnChangedHands?.Invoke();
    }

    public void ApplayDisposableCard(DisposableCardName disposableCard)
    {
        var so = disposableDic[disposableCard];
        var card = so.CreateCard();
        AddDisposableCard(so, card);
    }

    /// <summary>
    /// 상태이상 카드를 패에 추가한다.
    /// </summary>
    /// <param name="_statusCard">
    /// Card의 Suit는 StatusEffect로 지정한다. 숫자는 0으로 지정한다.
    /// 직접 호출하지 말고 StatusEffectCardSo에서 AddStatusEffect로 호출
    /// </param>
    /// <param name="IsUsable">사용가능 여부를 인자로 받는다.</param>
    public void AddStatusEffectCard(StatusEffectCardSO _statusCardInfo, MinorArcana _statusEffectCard)
    {
        Hand.Add(_statusEffectCard);
        IsUsableDic.Add(_statusEffectCard, _statusCardInfo.IsUsable);
        statusEffectCardDic[_statusEffectCard] = _statusCardInfo;
        OnChangedHands?.Invoke();
    }

    public void ApplyStatusEffect(StatusEffect type)
    {
        if (StatusEffectDic.TryGetValue(type, out var so))
        {
            var card = so.CreateCard();
            AddStatusEffectCard(so, card);
            Debug.Log($"[{so}] 상태이상 부여");
        }
        else
            Debug.LogError($"[{type}  so 없음]");
    }

    public void RemoveStatusEffectCard(MinorArcana statusEffectcard)
    {
        Hand.Remove(statusEffectcard);
        if (SelectedCard.Contains(statusEffectcard))
            SelectedCard.Remove(statusEffectcard);
        statusEffectCardDic.Remove(statusEffectcard);
        OnRemoveStatusEffectCard?.Invoke(statusEffectcard);
    }

    public void RemoveDisposableCard(MinorArcana disposablecard)
    {
        Hand.Remove(disposablecard);
        if (SelectedCard.Contains(disposablecard))
            SelectedCard.Remove(disposablecard);
        disposableCardDic.Remove(disposablecard);
    }


    #endregion

    #region 이벤트 관련 함수들
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
    public void ClearSelect()
    {
        SelectedCard.Clear();
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

    // TODO : 추후 턴종료 버리기 구현
    public IEnumerator TurnEndDiscard()
    {
        int handsCount = Hand.Count;
        if (MultiPleCardDic.Count != 0)
        {
            foreach (var KeyValue in MultiPleCardDic)
            {
                handsCount += KeyValue.Value - 1;
            }
        }

        int toDiscard = handsCount - drawNum;
        Debug.Log($"Hand {handsCount} Discard {toDiscard}");
        if (toDiscard > 0)
        {
            yield return StartCoroutine(HandleSelect(toDiscard));
        }
        else
        {
            ExitSelection();
        }
        // pseudocode : Discard(drawNum - handsCount);
    }

    public void EnterSelection(int DrawNum)
    {
        OnEnterSelectionMode?.Invoke(DrawNum);
    }

    public void ExitSelection()
    {
        OnExitSelectionMode?.Invoke();
    }

    public IEnumerator HandleSelect(int SelectNum)
    {
        EnterSelection(drawNum);

        ClearSelect();

        yield return new WaitUntil(() => SelectedCard.Count == SelectNum);

        ExitSelection();

        Discard(SelectedCard.ToList());
    }

    #endregion


    #region 디버프 함수들
    /// <summary>
    /// 카드에 디버프를 걸고, 필요 시 몬스터 스택 증폭을 연결
    /// </summary>
    public void ApplyDebuff(MinorArcana card, CardDebuffSO _debuff)
    {
        var so = _debuff;
        if (so == null) return;

        // 1) Debuff를 적용한다.
        Deck.Debuff(card, _debuff);

        // 2) Rust(유혹)일 경우 몬스터 스택 연결
        if (so is CharmSO charm)
        {
            // 유혹량이 발생할 때마다 스택 증가시키도록 바인딩
            charm.OnCharmCardUsed += amount =>
                LustMonster.Instance.IncreaseLustStack(amount);
        }
    }
    /// <summary>
    /// 카드에 디버프를 걸고, 필요 시 몬스터 스택 증폭을 연결
    /// </summary>
    public void ApplyDebuff(MinorArcana card, CardDebuff _debuff)
    {
        var debuffSO = GetDebuffSO(_debuff);
        if (debuffSO == null) return;

        // 1) Debuff를 적용한다.
        Deck.Debuff(card, debuffSO);

        // 2) Rust(유혹)일 경우 몬스터 스택 연결
        if (debuffSO is CharmSO charm)
        {
            // 유혹량이 발생할 때마다 스택 증가시키도록 바인딩
            charm.OnCharmCardUsed += amount =>
                LustMonster.Instance.IncreaseLustStack(amount);
        }
    }

    // /// <summary>
    // /// 배틀 종료 시(혹은 카드 제거 시) 디버프 해제
    // /// </summary>
    // public void RemoveDebuff(MinorArcana card)
    // {
    //     var type = card.debuff.debuffinfo;
    //     var so = DebuffDatabase.Instance.GetDebuffSO(type);
    //     if (so != null)
    //         so.OnUnSubscribe(card, this);
    //     card.debuff.DebuffToCard(CardDebuff.none);
    // }

    public void SetBattleBonusList(CardBonus cardBonus, BonusType type, float Amount)
    {
        if (type == BonusType.Bonus)
            BattleBonusDic[cardBonus].Add(Amount);
        else
            BattlePenaltyDic[cardBonus].Add(Amount);
    }

    public void SetTurnBonusList(CardBonus cardBonus, BonusType type, float Amount)
    {
        if (type == BonusType.Bonus)
            TurnBonusDic[cardBonus].Add(Amount);
        else
            TurnPenaltyDic[cardBonus].Add(Amount);
    }
    #endregion




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