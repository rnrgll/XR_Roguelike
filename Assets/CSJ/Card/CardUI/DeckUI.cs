using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] Button DeckButton;
    [SerializeField] TextMeshProUGUI DeckCount;
    [SerializeField] CardController cardController;
    [SerializeField] BattleDeckUI BattleDeckUI;

    private void OnEnable()
    {
        cardController.OnChangedHands += OnDrawCard;
        DeckButton.onClick.AddListener(BattleDeckUI.OpenPanel);
    }
    private void OnDisable()
    {
        cardController.OnChangedHands -= OnDrawCard;
        DeckButton.onClick.RemoveListener(BattleDeckUI.OpenPanel);
    }


    private void OnDrawCard()
    {
        DeckCount.text = $"{cardController.BattleDeck.Count} / {cardController.DeckPile.Count}";
    }
}
