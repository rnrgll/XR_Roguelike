using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheTower/Reversed")]
public class TheTowerReversedAbility : ScriptableObject, IArcanaAbility
{
    public void Excute(ArcanaContext ctx)
    {

        var controller = ctx.cardController;
        controller.Draw(5);
        // TODO : UI와 연계
        List<MinorArcana> handList = controller.Hand.GetCardList().GetRange(0, 5);
        controller.Discard(handList);
    }
}