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

// TODO : 추후 기획에 맞춰 조정
public enum CardEnchant
{
    normal, Upgraded, Maximum
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