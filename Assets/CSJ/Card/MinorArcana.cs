using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct MinorArcana : ICard
{
    public string CardName { get; }
    public MinorSuit CardSuit { get; }
    public int CardNum { get; }


    public MinorArcana(string _name, MinorSuit _suit, int _num)
    {
        CardName = _name;
        CardSuit = _suit;
        CardNum = _num;
    }
}
