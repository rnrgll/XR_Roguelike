using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandNumUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textArea;
    private CardController cardController;

    private void OnEnable()
    {
        cardController = FindAnyObjectByType<PlayerController>().GetCardController();
        cardController.OnChangedHands += RefreshCount;
        cardController = TurnManager.Instance.GetPlayerController().GetCardController();
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
