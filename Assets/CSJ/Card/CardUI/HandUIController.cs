using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HandUIController : MonoBehaviour
{
    [SerializeField] private RectTransform handContainer;
    [SerializeField] private GameObject cardPrefab;
    private CardController cardController;
    private List<GameObject> spawnedCards = new List<GameObject>();
    public event Action OnCardSetted;

    private void InitializeUI(CardController cc)
    {
        if (cardController != null)
        {
            cardController.OnSelectionChanged -= SyncUI;
            cardController.OnChangedHands -= RefreshHand;
        }

        cardController = cc;
        cardController.OnChangedHands += RefreshHand;
        cardController.OnSelectionChanged += SyncUI;

        RefreshHand();
    }

    private void OnDisable()
    {
        cardController.OnChangedHands -= RefreshHand;
        cardController.OnSelectionChanged -= SyncUI;
    }

    public void RefreshHand()
    {
        if (cardController == null) return;

        foreach (var go in spawnedCards)
        {
            Destroy(go);
        }
        spawnedCards.Clear();

        cardController.SortByStand();
        var hand = cardController.GetHand();

        foreach (var card in hand)
        {
            var go = Instantiate(cardPrefab, handContainer);
            var ui = go.GetComponent<CardUI>();
            ui.Setup(card);

            ui.OnClick += c =>
            {
                if (cardController.IsUsableDic.ContainsKey(c) &&
                cardController.IsUsableDic[c] == false) return;

                if (cardController.SelectedCard.Count >= 5)
                {
                    if (ui._isSelected)
                    {
                        ui.ToggleSelect();
                        CardDeSelected(c);
                    }
                    return;
                }

                bool now = ui.ToggleSelect();
                if (now) CardSelected(c);
                else CardDeSelected(c);
            };
            spawnedCards.Add(go);
        }
        OnCardSetted?.Invoke();
    }

    private void CardSelected(MinorArcana card)
    {
        cardController.OnCardSelected?.Invoke(card);
    }

    private void CardDeSelected(MinorArcana card)
    {
        cardController.OnCardDeSelected?.Invoke(card);
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
