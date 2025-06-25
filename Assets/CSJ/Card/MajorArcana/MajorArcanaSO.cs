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

}
