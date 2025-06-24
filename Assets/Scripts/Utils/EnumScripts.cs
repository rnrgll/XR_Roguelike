using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinorSuit
{
    Wands, Cups, Swords, Pentacles
}

public enum MajorPosition
{
    Normal, Reverse
}

public enum CardStatus { DeckList, BattleDeck, Hand, Graveyard }

public enum CardCombinationEnum
{
    HighCard,
    OnePair,
    TwoPair,
    Triple,
    Straight,
    Flush,
    FullHouse,
    FourCard,
    StraightFlush,
    FiveCard,
    FiveJoker
}

namespace Map
{
    public enum NodeType
    {
        NotAssgined,
        Battle, // 전투
        Shop, // 상점
        Event, // 이벤트
        Boss, // 보스
    }
    
    public enum NodeState
    {
        Locked,
        Visited,
        Attainable
    }
}
