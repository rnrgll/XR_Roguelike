using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/MajorArcana", fileName = "NewMajorArcana")]
public class MajorArcanaSO : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("카드의 이름")]
    public int cardName;

    [Tooltip("카드의 설명")]
    [TextArea]
    public string desctiption;

    [Header("스프라이트")]
    public Sprite sprite;

    [Header("능력(정위치, 역위치)")]
    public ScriptableObject uprightAbilitySO;
    public ScriptableObject reversedAbilitySO;

    private IArcanaAbility UprightAbilitySO => uprightAbilitySO as IArcanaAbility;
    private IArcanaAbility ReversedAbilitySO => reversedAbilitySO as IArcanaAbility;

    public MajorPosition cardPos { get; private set; }
    private MajorPosition PrevPos;

    public void Activate(GameObject _go)
    {
        ArcanaContext ctx = new ArcanaContext { Owner = _go, MajorPos = cardPos };
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
}
