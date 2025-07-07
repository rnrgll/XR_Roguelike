using CardEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CombinationUI : UIRequire
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI sumText;

    public override void InitializeUI(PlayerController pc)
    {
        base.InitializeUI(pc);
        RefreshUI();
    }

    protected override void Subscribe()
    {
        cardController.OnSelectionChanged += RefreshUI;
    }

    protected override void UnSubscribe()
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
        sumText.text = $"{cardController.sumofNums} * {cardController.ComboMultDic[_comb]}";
    }

    private void RefreshUI()
    {
        comboText.text = "";
        sumText.text = "";
    }

}
