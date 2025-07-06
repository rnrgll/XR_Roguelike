using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BattleDeckUI : UIRequire
{
    [SerializeField] private HandUIController handUI;
    [SerializeField] private RectTransform CupsHorizon;
    [SerializeField] private RectTransform SwordsHorizon;
    [SerializeField] private RectTransform PentaclesHorizon;
    [SerializeField] private RectTransform WandsHorizon;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private GameObject BattleDeckCanvas;
    [SerializeField] private Button BackGround;
    [SerializeField] private Button CloseButton;


    private List<GameObject> spawnedCards = new List<GameObject>();
    private bool isSetted = false;

    private void Awake()
    {
        BattleDeckCanvas.SetActive(false);
        handUI.OnCardSetted += isSet;
    }

    protected override void Subscribe()
    {
        BackGround.onClick.AddListener(ClosePanel);
        CloseButton.onClick.AddListener(ClosePanel);
    }

    protected override void UnSubscribe()
    {
        BackGround.onClick.RemoveListener(ClosePanel);
        CloseButton.onClick.RemoveListener(ClosePanel);
    }

    public void OpenPanel()
    {
        if (!isSetted) return;
        BattleDeckCanvas.SetActive(true);
        RefreshPanel();
    }

    private void ClosePanel()
    {
        BattleDeckCanvas.SetActive(false);
    }

    public void RefreshPanel()
    {
        if (cardController == null) return;
        foreach (var go in spawnedCards)
        {
            Destroy(go);
        }
        spawnedCards.Clear();

        cardController.SortByStand();
        var Deck = cardController.DeckPile.GetCardList();
        List<MinorArcana> UsedCard = cardController.Hand.GetCardList();
        UsedCard.AddRange(cardController.Graveyard.GetCardList());

        foreach (var card in Deck)
        {
            var suitOfCard = card.CardSuit;
            GameObject go = null;
            switch (suitOfCard)
            {
                case CardEnum.MinorSuit.Wands:
                    go = Instantiate(cardPrefab, WandsHorizon);
                    break;
                case CardEnum.MinorSuit.Cups:
                    go = Instantiate(cardPrefab, CupsHorizon);
                    break;
                case CardEnum.MinorSuit.Swords:
                    go = Instantiate(cardPrefab, SwordsHorizon);
                    break;
                case CardEnum.MinorSuit.Pentacles:
                    go = Instantiate(cardPrefab, PentaclesHorizon);
                    break;
            }
            var ui = go.GetComponent<MiniCardUI>();
            ui.Setup(card);
            ui.MarkUsed(UsedCard.Contains(card));

            spawnedCards.Add(go);
        }
    }

    public void isSet()
    {
        isSetted = true;
        handUI.OnCardSetted -= isSet;
    }
}
