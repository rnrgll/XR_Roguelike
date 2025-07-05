using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(fileName = "TheTowerReversed", menuName = "Tarot/Abilities/TheTower/Reversed")]
public class TheTowerReversedAbility : ScriptableObject, IArcanaAbility
{
    [SerializeField] int DrawNum = 8;
    public void Excute(ArcanaContext ctx)
    {
        var controller = ctx.player.GetCardController();
        controller.Draw(DrawNum);
        // TODO : UI와 연계
        HandleSelect(controller);
    }

    public IEnumerator HandleSelect(CardController cardController)
    {
        cardController.EnterSelection(DrawNum);

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

        cardController.OnSelectionChanged -= selectionWatcher;
        cardController.ExitSelection();

        cardController.Discard(cardController.SelectedCard);

        cardController.OnChangedHands?.Invoke();
    }
}