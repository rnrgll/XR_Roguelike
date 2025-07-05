using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitUI : MonoBehaviour
{
    [SerializeField] private Button attackButton;
    [SerializeField] private Button DiscardButton;
    [SerializeField] private Button SuitSortButton;
    [SerializeField] private Button NumSortButton;
    private CardController cardController;

    private void OnEnable()
    {
        cardController = FindAnyObjectByType<PlayerController>().GetCardController();
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        DiscardButton.onClick.AddListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.AddListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.AddListener(OnNumSortButtonClicked);

        cardController.OnSelectionChanged += _ => UpdateButtons();
        UpdateButtons();
    }

    private void OnDisable()
    {
        attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        DiscardButton.onClick.RemoveListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.RemoveListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.RemoveListener(OnNumSortButtonClicked);

        cardController.OnSelectionChanged -= _ => UpdateButtons();
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
