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
    private Action<int> _onUnactive;

    protected override void Subscribe()
    {
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        DiscardButton.onClick.AddListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.AddListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.AddListener(OnNumSortButtonClicked);
        TurnEndButton.onClick.AddListener(OnTurnEndButtonClicked);

        _onSelectionChanged = _ => UpdateButtons();
        _onUnactive = _ => UnactiveButton();
        cardController.OnSelectionChanged += _onSelectionChanged;
        cardController.OnEnterSelectionMode += _onUnactive;
        cardController.OnExitSelectionMode += ActiveButton;

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
        cardController.OnEnterSelectionMode -= _onUnactive;
        cardController.OnExitSelectionMode -= ActiveButton;
        _onSelectionChanged = null;
        _onUnactive = null;
    }

    private void UpdateButtons()
    {
        if (attackButton == null) return;
        int selectedCount = cardController.SelectedCard.Count;

        attackButton.interactable = selectedCount >= 5;
        if (cardController.DiscardCount < 1)
            DiscardButton.interactable = false;
        else
            DiscardButton.interactable = selectedCount >= 1;
    }

    private void ActiveButton()
    {
        cardController.OnSelectionChanged += _onSelectionChanged;

        attackButton.interactable = true;
        SuitSortButton.interactable = true;
        NumSortButton.interactable = true;
        TurnEndButton.interactable = true;
        if (cardController.DiscardCount < 1)
            DiscardButton.interactable = false;
        else
            DiscardButton.interactable = true;
    }

    private void UnactiveButton()
    {
        cardController.OnSelectionChanged -= _onSelectionChanged;

        attackButton.interactable = false;
        DiscardButton.interactable = false;
        SuitSortButton.interactable = false;
        NumSortButton.interactable = false;
        TurnEndButton.interactable = false;
    }

    private void OnAttackButtonClicked()
    {
        cardController.Submit();
    }

    private void OnDiscardButtonClicked()
    {
        cardController.DiscardCount--;
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
        UnactiveButton();
        playerController.OnTurnStarted += OnTurnStart;
        playerController.StartCoroutine(playerController.EndTurn());
    }

    private void OnTurnStart()
    {
        ActiveButton();
        playerController.OnTurnStarted -= OnTurnStart;
    }
}
