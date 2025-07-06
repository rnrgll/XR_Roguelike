using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeckUI : UIRequire
{
    [SerializeField] Button DeckButton;
    [SerializeField] TextMeshProUGUI DeckCount;
    [SerializeField] BattleDeckUI BattleDeckUI;
    [SerializeField] TarotDeckUI tarotDeck;
    private UnityAction MajorActive;

    protected override void Subscribe()
    {
        cardController.OnChangedHands += OnDrawCard;
        MajorActive = () => tarotDeck.SetActive(false);
        DeckButton.onClick.AddListener(MajorActive);
        DeckButton.onClick.AddListener(BattleDeckUI.OpenPanel);
    }

    protected override void UnSubscribe()
    {
        cardController.OnChangedHands -= OnDrawCard;
        DeckButton.onClick.RemoveListener(MajorActive);
        DeckButton.onClick.RemoveListener(BattleDeckUI.OpenPanel);
    }


    private void OnDrawCard()
    {
        DeckCount.text = $"{cardController.BattleDeck.Count} / {cardController.DeckPile.Count}";
    }
}
