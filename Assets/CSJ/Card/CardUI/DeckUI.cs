using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : UIRequire
{
    [SerializeField] Button DeckButton;
    [SerializeField] TextMeshProUGUI DeckCount;
    [SerializeField] BattleDeckUI BattleDeckUI;

    protected override void Subscribe()
    {
        cardController.OnChangedHands += OnDrawCard;
        DeckButton.onClick.AddListener(BattleDeckUI.OpenPanel);
    }

    protected override void UnSubscribe()
    {
        cardController.OnChangedHands -= OnDrawCard;
        DeckButton.onClick.RemoveListener(BattleDeckUI.OpenPanel);
    }


    private void OnDrawCard()
    {
        DeckCount.text = $"{cardController.BattleDeck.Count} / {cardController.DeckPile.Count}";
    }
}
