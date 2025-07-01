using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

// 게임의 주된 플레이를 담당하는 마이너 아르카나 구조체
// ICard 인터페이스를 상속하며, 읽기 전용으로 선언되어 있다.
// 카드 이름, 카드 문양, 카드 숫자, 인챈트로 구성되어 있다.
public readonly struct MinorArcana : ICard
{
    // 카드 이름
    public string CardName { get; }
    // 카드 문양
    public MinorSuit CardSuit { get; }
    // 카드 숫자
    public int CardNum { get; }
    // 카드 인챈트
    public Enchant Enchant { get; }


    // 마이너 아르카나 생성자, 인챈트는 자동으로 none으로 생성된다.
    public MinorArcana(string _name, MinorSuit _suit, int _num)
    {
        CardName = _name;
        CardSuit = _suit;
        CardNum = _num;
        Enchant = new();
    }
}
