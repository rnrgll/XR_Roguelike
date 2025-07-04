using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheWorld/Upright")]
public class TheWorldUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.cardController;
        // TODO : 추후 제대로 구현


    }
}