using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CardEnum;

public class SubmitUI : UIRequire
{
    [SerializeField] private Button attackButton;
    [SerializeField] private Button DiscardButton;
    [SerializeField] private Button SuitSortButton;
    [SerializeField] private Button NumSortButton;
    [SerializeField] private Button TurnEndButton;
    private Action<CardCombinationEnum> _onSelectionChanged;

    protected override void Subscribe()
    {
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        DiscardButton.onClick.AddListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.AddListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.AddListener(OnNumSortButtonClicked);
        TurnEndButton.onClick.AddListener(OnTurnEndButtonClicked);

        _onSelectionChanged = _ => UpdateButtons();
        cardController.OnSelectionChanged += _onSelectionChanged;
        UpdateButtons();
    }

    protected override void UnSubscribe()
    {
        attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        DiscardButton.onClick.RemoveListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.RemoveListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.RemoveListener(OnNumSortButtonClicked);
        TurnEndButton.onClick.RemoveListener(OnTurnEndButtonClicked);

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

    private void OnTurnEndButtonClicked()
    {
        playerController.EndTurn();
    }
}
