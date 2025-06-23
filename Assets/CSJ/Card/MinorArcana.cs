using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct MinorArcana : ICard
{
    public MinorSuit CardSuit { get; }
    public int CardNum { get; }
    // order는 카드가 여러 장일때 카드를 구분하도록 순서를 나타냄
    // 현재는 사용하지 않지만 추후 확장을 대비해 넣음
    public int CardOrder { get; }


    public MinorArcana(MinorSuit _suit, int _num, int _order = 1)
    {
        CardSuit = _suit;
        CardNum = _num;
        CardOrder = _order;
    }

}
