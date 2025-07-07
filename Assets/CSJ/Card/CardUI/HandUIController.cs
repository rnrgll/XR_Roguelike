using CardEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class HandUIController : UIRequire
{
    [SerializeField] private RectTransform handContainer;
    [SerializeField] private GameObject cardPrefab;
    private List<GameObject> spawnedCards = new List<GameObject>();
    public event Action OnCardSetted;

    public override void InitializeUI(PlayerController pc)
    {
        base.InitializeUI(pc);
        RefreshHand();
    }

    protected override void Subscribe()
    {
        cardController.OnChangedHands += RefreshHand;
        cardController.OnSelectionChanged += SyncUI;
    }

    protected override void UnSubscribe()
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

// public class HandUIController : UIRequire
// {
//     [SerializeField] private RectTransform handContainer;
//     [SerializeField] private GameObject cardPrefab;
//     private List<GameObject> spawnedCards = new List<GameObject>();
//     public Action OnCardSetted;
//     public Action<MinorArcana> RefreshAction;
// 
//     public override void InitializeUI(PlayerController pc)
//     {
//         base.InitializeUI(pc);
//         RefreshHand();
//     }
// 
//     protected override void Subscribe()
//     {
//         RefreshAction = _ => RefreshHand();
//         cardController.OnChangedHands += RefreshHand;
//         cardController.OnSelectionChanged += SyncUI;
//         cardController.OnCardSelected += RefreshAction;
//         cardController.OnCardDeSelected += RefreshAction;
//     }
// 
//     protected override void UnSubscribe()
//     {
//         cardController.OnChangedHands -= RefreshHand;
//         cardController.OnSelectionChanged -= SyncUI;
//         cardController.OnCardSelected -= RefreshAction;
//         cardController.OnCardDeSelected -= RefreshAction;
//     }
// 
//     public void RefreshHand()
//     {
//         if (cardController == null) return;
//         foreach (var go in spawnedCards)
//         {
//             Destroy(go);
//         }
//         spawnedCards.Clear();
// 
//         cardController.SortByStand();
//         var hand = cardController.GetHand();
//         foreach (var card in hand)
//         {
//             if (cardController.SelectedCard.Contains(card)) continue;
//             var go = Instantiate(cardPrefab, handContainer);
//             var ui = go.GetComponent<CardUI>();
//             ui.Setup(card);
// 
// 
//             ui.OnClick += c =>
//             {
// 
//                 if (cardController.IsUsableDic.TryGetValue(c, out var usable) &&
//                 !usable) return;
// 
//                 if (cardController.SelectedCard.Count >= 5) return;
// 
//                 bool now = ui.ToggleSelect();
//                 if (now) CardSelected(c);
//                 else CardDeSelected(c);
//             };
//             spawnedCards.Add(go);
//         }
//         OnCardSetted?.Invoke();
//     }
// 
//     private void CardSelected(MinorArcana card)
//     {
//         cardController.OnCardSelected?.Invoke(card);
//     }
//     private void CardDeSelected(MinorArcana card)
//     {
//         cardController.OnCardDeSelected?.Invoke(card);
//     }
// 
//     private void SyncUI(CardCombinationEnum _)
//     {
//         foreach (var go in spawnedCards)
//         {
//             var ui = go.GetComponent<CardUI>();
//             bool shouldBeSelected = cardController.SelectedCard.Contains(ui.CardData);
//             ui.SyncSelected(shouldBeSelected);
//         }
//     }
// }
