using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] Button DeckButton;
    [SerializeField] TextMeshProUGUI DeckCount;
    [SerializeField] BattleDeckUI BattleDeckUI;
    private CardController cardController;


    private void InitializeUI(CardController cc)
    {
        if (cardController != null)
        {
            cardController.OnChangedHands -= OnDrawCard;
            DeckButton.onClick.RemoveListener(BattleDeckUI.OpenPanel);
        }

        cardController = cc;
        cardController.OnChangedHands += OnDrawCard;
        DeckButton.onClick.AddListener(BattleDeckUI.OpenPanel);
    }

    private void OnDestroy()
    {
        cardController.OnChangedHands -= OnDrawCard;
        DeckButton.onClick.RemoveListener(BattleDeckUI.OpenPanel);
    }


    private void OnDrawCard()
    {
        DeckCount.text = $"{cardController.BattleDeck.Count} / {cardController.DeckPile.Count}";
    }
}
