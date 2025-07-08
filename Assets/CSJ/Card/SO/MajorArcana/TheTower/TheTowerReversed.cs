using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "TheTowerReversed", menuName = "Tarot/Abilities/TheTower/Reversed")]
public class TheTowerReversedAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] int DrawNum = 5;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.player.GetCardController();
        controller.Draw(DrawNum);
        // TODO : UI와 연계
        controller.StartCoroutine(controller.HandleSelect(DrawNum));
    }
}