using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajorArcana : ICard
{
    public string ArcanaName;
    public MajorPosition cardPosition { get; private set; }

    public MajorArcana(string _name)
    {
        cardPosition = MajorPosition.Normal;
        ArcanaName = _name;

        //TODO : 메이저 아르카나의 효과를 csv로 받아 해당 내용을 해당 메이저 아르카나에 삽입
        // 스크립터블 오브젝트로 구현?
    }



}
