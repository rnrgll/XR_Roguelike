using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandNumUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textArea;
    private CardController cardController;

    private void InitializeUI(CardController cc)
    {
        if (cardController != null)
            cardController.OnChangedHands -= RefreshCount;

        cardController = cc;
        cardController.OnChangedHands += RefreshCount;
    }

    private void OnDisable()
    {
        cardController.OnChangedHands -= RefreshCount;
    }

    private void RefreshCount()
    {
        int count = cardController.Hand.Count;
        textArea.text = $"{count} / {cardController.drawNum}";
    }
}
