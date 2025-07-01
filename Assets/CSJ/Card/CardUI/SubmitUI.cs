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
    [SerializeField] private CardController cardController;

    private void OnEnable()
    {
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        DiscardButton.onClick.AddListener(OnDiscardButtonClicked);
        SuitSortButton.onClick.AddListener(OnSuitSortButtonClicked);
        NumSortButton.onClick.AddListener(OnNumSortButtonClicked);
    }

    private void OnDisable()
    {
        attackButton.onClick.RemoveListener(OnAttackButtonClicked);
        DiscardButton.onClick.RemoveListener(OnDiscardButtonClicked);
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
