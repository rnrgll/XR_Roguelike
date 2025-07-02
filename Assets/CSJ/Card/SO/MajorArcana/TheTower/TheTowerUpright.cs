using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheTower/Upright")]
public class TheTowerUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {

        var controller = ctx.Owner.GetComponent<CardController>();

        controller.Discard(controller.Hand.GetCardList());
        controller.Draw(8);
    }
}