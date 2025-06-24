
using manager = Managers.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MajorArcana : ICard
{
    public string ArcanaName;
    public MajorPosition cardPos { get; private set; }
    public MajorPosition PrevPos;

    public MajorArcana(string _name)
    {
        cardPos = MajorPosition.Normal;
        ArcanaName = _name;

        //TODO : 메이저 아르카나의 효과를 csv로 받아 해당 내용을 해당 메이저 아르카나에 삽입
        // 스크립터블 오브젝트로 구현?
    }

    /// <summary>
    /// 메이저 카드의 회전을 결정한다.
    /// 정위치와 역위치를 랜덤하게 정해준다
    /// </summary>
    /// <returns> 카드의 방향이 이전과 바뀌었는지를 bool로 리턴한다.</returns>
    public bool CardRotation()
    {
        PrevPos = cardPos;

        MajorPosition nowPos = manager.randomManager.RandomPosition();
        if (PrevPos == nowPos) return false;

        cardPos = nowPos;
        return true;
    }



}
