using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CardEnum;

public class SubmitUI : MonoBehaviour
{
    [SerializeField] private Button attackButton;
    [SerializeField] private Button DiscardButton;
    [SerializeField] private Button SuitSortButton;
    [SerializeField] private Button NumSortButton;
    private CardController cardController;
    private Action<CardCombinationEnum> _onSelectionChanged;

    private void InitializeUI(CardController cc)
    {
        cardController = cc;
    }

    private void OnEnable()
    {
        if (cardController == null) return;


        attackButton.onClick.AddListener(OnAttackButtonClicked);
        DiscardButton.onClick.AddListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.AddListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.AddListener(OnNumSortButtonClicked);

        _onSelectionChanged = _ => UpdateButtons();
        cardController.OnSelectionChanged += _onSelectionChanged;
        UpdateButtons();
    }

    private void OnDisable()
    {
        attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        DiscardButton.onClick.RemoveListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.RemoveListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.RemoveListener(OnNumSortButtonClicked);

        cardController.OnSelectionChanged -= _onSelectionChanged;
        _onSelectionChanged = null;
    }

    private void UpdateButtons()
    {
        int selectedCount = cardController.SelectedCard.Count;

        attackButton.interactable = selectedCount >= 5;
        DiscardButton.interactable = selectedCount >= 1;
    }

    private void OnAttackButtonClicked()
    {
        cardController.Submit();
    }

    private void OnDiscardButtonClicked()
    {
        cardController.exchangeHand(cardController.SelectedCard);
    }

    private void OnSuitSortButtonClicked()
    {
        cardController.ChangeStandBySuit();
    }
    private void OnNumSortButtonClicked()
    {
        cardController.ChangeStandByNum();
    }
}
