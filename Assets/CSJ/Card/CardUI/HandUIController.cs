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
    [SerializeField] private CardController cardController;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void OnEnable()
    {
        cardController.OnChangedHands += RefreshHand;
    }

    private void OnDisable()
    {
        cardController.OnChangedHands -= RefreshHand;
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

                bool now = ui.ToggleSelect();
                if (now) CardSelected(c);
                else CardDeSelected(c);
            };
            spawnedCards.Add(go);
        }
    }

    private void CardSelected(MinorArcana card)
    {
        cardController.OnCardSelected?.Invoke(card);
    }

    private void CardDeSelected(MinorArcana card)
    {
        cardController.OnCardDeSelected?.Invoke(card);
    }
}
