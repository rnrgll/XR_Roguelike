using CardEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombinationUI : MonoBehaviour
{
    [SerializeField] private CardController cardController;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI sumText;

    private void Awake()
    {
        RefreshUI();
    }
    private void OnEnable()
    {
        cardController.OnSelectionChanged += RefreshUI;
    }

    private void OnDisable()
    {
        cardController.OnSelectionChanged -= RefreshUI;
    }

    private void RefreshUI(CardCombinationEnum _comb)
    {
        if (cardController.GetSelection().Count == 0)
        {
            comboText.text = "";
            sumText.text = "";
            return;
        }
        comboText.text = _comb.ToString();
        sumText.text = cardController.sumofNums.ToString();
    }

    private void RefreshUI()
    {
        comboText.text = "";
        sumText.text = "";
    }

}
