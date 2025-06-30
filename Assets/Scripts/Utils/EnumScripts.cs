using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardEnum
{
    public enum MinorSuit { Wands, Cups, Swords, Pentacles, wildCard = 30, Special = 99 }

    public enum MajorPosition { Upright, Reversed }

    // TODO : 추후 기획에 맞춰 조정
    public enum CardEnchant { none, Bonus, Mult, Wild, Glass, Steel, Gold, Lucky }

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

namespace InGameShop
{
    public enum SortOrder
    {
        Item= 3,
        PopUp = 5,
        
    }

    public enum ButtonState
    {
        Active,
        Deactive,
    }
    
}
