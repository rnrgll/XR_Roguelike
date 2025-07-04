using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(menuName = "Tarot/MajorArcana", fileName = "NewMajorArcana")]
public class MajorArcanaSO : ScriptableObject
{

    [Header("기본 정보")]
    [Tooltip("카드의 이름")]
    public string cardName;

    [Header("스프라이트")]
    public Sprite sprite;

    [Header("능력(정위치, 역위치)")]
    public ScriptableObject uprightAbilitySO;
    public ScriptableObject reversedAbilitySO;

    private IArcanaAbility UprightAbilitySO => uprightAbilitySO as IArcanaAbility;
    private IArcanaAbility ReversedAbilitySO => reversedAbilitySO as IArcanaAbility;

    public MajorPosition cardPos { get; private set; }
    private MajorPosition PrevPos;
    protected PlayerController playercontroller = TurnManager.Instance.GetPlayerController();


    public void Activate()
    {
        ArcanaContext ctx = new ArcanaContext { player = playercontroller, MajorPos = cardPos };
        if (cardPos == MajorPosition.Upright)
            UprightAbilitySO?.Excute(ctx);
        else
            ReversedAbilitySO?.Excute(ctx);
    }

    public bool CardRotation()
    {
        PrevPos = cardPos;

        MajorPosition nowPos = Manager.randomManager.RandomPosition();
        if (PrevPos == nowPos) return false;

        cardPos = nowPos;
        return true;
    }

    //TODO : 애니메이터와 연계, 추후 스프라이트를 뒤집는 기능 추가
    public void Rotate()
    {
        PrevPos = cardPos;
        cardPos ^= MajorPosition.Reversed;
    }
}
