using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(fileName = "TheTowerUpright", menuName = "Tarot/Abilities/TheTower/Upright")]
public class TheTowerUprightAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {
        var cardController = ctx.player.GetCardController();
        cardController.exchangeHand(cardController.Hand.GetCardList());
    }
}