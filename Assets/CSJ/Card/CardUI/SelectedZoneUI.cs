using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedZoneUI : UIRequire
{
    [SerializeField] private RectTransform SelectedZone;
    [SerializeField] private GameObject cardPrefab;
    private List<GameObject> spawnedCards = new List<GameObject>();
    private List<MinorArcana> _selectedCards = new List<MinorArcana>();

    public override void InitializeUI(PlayerController pc)
    {
        base.InitializeUI(pc);
        RefreshHand();
    }

    protected override void Subscribe()
    {
        cardController.OnChangedHands += RefreshHand;
        cardController.OnSelectionChanged += SyncUI;
        cardController.OnCardDeSelected += AddCard;
    }

    protected override void UnSubscribe()
    {
        cardController.OnChangedHands -= RefreshHand;
        cardController.OnSelectionChanged -= SyncUI;
        cardController.OnCardDeSelected -= AddCard;
    }

    public void RefreshHand()
    {
        if (cardController == null) return;
        Debug.Log("selectedZone");
        foreach (var go in spawnedCards)
        {
            Destroy(go);
        }
        spawnedCards.Clear();

        var selectedCard = new List<MinorArcana>(_selectedCards);
        foreach (var card in selectedCard)
        {

            var go = Instantiate(cardPrefab, SelectedZone);
            var ui = go.GetComponent<CardUI>();
            ui.Setup(card);

            ui.OnClick += c =>
            {
                Debug.Log("cardDeselect");
                ui.ToggleSelect();
                CardDeSelected(c);
            };
            spawnedCards.Add(go);
        }
    }

    private void CardDeSelected(MinorArcana card)
    {
        _selectedCards.Remove(card);
        RefreshHand();
        cardController.OnCardDeSelected?.Invoke(card);
    }

    private void AddCard(MinorArcana card)
    {
        _selectedCards.Add(card);
        RefreshHand();
    }

    private void SyncUI(CardCombinationEnum _)
    {
        foreach (var go in spawnedCards)
        {
            var ui = go.GetComponent<CardUI>();
            bool shouldBeSelected = cardController.SelectedCard.Contains(ui.CardData);
            ui.SyncSelected(shouldBeSelected);
        }
    }
}