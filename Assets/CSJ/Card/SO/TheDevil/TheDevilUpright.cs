using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheDevil/Upright")]
public class TheDevilUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.Owner.GetComponent<CardController>();
        // TODO : 추후 제대로 구현


    }
}