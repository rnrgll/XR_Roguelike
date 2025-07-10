using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TheTowerUpright", menuName = "Tarot/Abilities/TheTower/Upright")]
public class TheTowerUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        var cardController = ctx.player.GetCardController();
        cardController.exchangeHand(new List<MinorArcana>(cardController.Hand.GetCardList()));
    }
}