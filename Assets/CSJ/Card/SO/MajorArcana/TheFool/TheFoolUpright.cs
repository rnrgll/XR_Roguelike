using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheFool/Upright")]
public class TheFoolUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        MinorArcana Joker = new MinorArcana("disposJoker", MinorSuit.wildCard, 14);
        ctx.cardController.
        AddDisposableCard(Joker);
    }
}

