using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct MinorArcana : ICard
{
    public MinorSuit CardSuit { get; }
    public int CardNum { get; }


    public MinorArcana(MinorSuit _suit, int _num)
    {
        CardSuit = _suit;
        CardNum = _num;
    }
}
