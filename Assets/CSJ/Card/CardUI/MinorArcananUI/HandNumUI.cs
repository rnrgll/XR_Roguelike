using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandNumUI : UIRequire
{
    [SerializeField] TextMeshProUGUI textArea;
    private Action<MinorArcana> OnDiscard;

    protected override void Subscribe()
    {
        OnDiscard = _ => RefreshCount();
        cardController.OnChangedHands += RefreshCount;
        cardController.OnCardDiscarded += OnDiscard;
    }

    protected override void UnSubscribe()
    {
        cardController.OnChangedHands -= RefreshCount;
        cardController.OnCardDiscarded -= OnDiscard;
        OnDiscard = null;
    }

    private void RefreshCount()
    {
        Debug.Log(cardController.DiscardCount);
        int count = cardController.Hand.Count;
        textArea.text = $"{count} / {cardController.drawNum}\nDisCard : {cardController.DiscardCount}";
    }
}
