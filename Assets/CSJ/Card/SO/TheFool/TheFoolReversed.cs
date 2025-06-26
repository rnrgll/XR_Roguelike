using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheFool/Reversed")]
public class TheFoolReversedAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        MinorArcana Joker = new MinorArcana("disposJoker", MinorSuit.wildCard, 14);
        ctx.Owner.GetComponent<CardController>().
        AddDisposableCard(Joker);
    }
}
