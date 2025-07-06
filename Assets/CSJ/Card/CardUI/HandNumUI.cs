using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandNumUI : UIRequire
{
    [SerializeField] TextMeshProUGUI textArea;

    protected override void Subscribe()
    {
        cardController.OnChangedHands += RefreshCount;
    }

    protected override void UnSubscribe()
    {
        cardController.OnChangedHands -= RefreshCount;
    }

    private void RefreshCount()
    {
        int count = cardController.Hand.Count;
        textArea.text = $"{count} / {cardController.drawNum}";
    }
}
