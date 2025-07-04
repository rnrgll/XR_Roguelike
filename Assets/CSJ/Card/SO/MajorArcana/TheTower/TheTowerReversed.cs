using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(menuName = "Tarot/Abilities/TheTower/Reversed")]
public class TheTowerReversedAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] int DrawNum;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.player.GetCardController();
        controller.Draw(DrawNum);
        // TODO : UI와 연계
        HandleSelect(controller);
        //controller.Discard(handList);
    }

    public IEnumerator HandleSelect(CardController cardController)
    {
        //OnEnterSelectionMode?.Invoke(DrawNum);

        cardController.ClearSelect();

        bool done = false;
        Action<CardCombinationEnum> selectionWatcher = _ =>
        {
            if (cardController.SelectedCard.Count == 5)
                done = true;
        };
        cardController.OnSelectionChanged += selectionWatcher;

        while (!done)
            yield return null;

       //onSel
    }
}