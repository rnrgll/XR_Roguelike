using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RootingUIController : UIRequire
{
    [SerializeField] GameObject RootingUI;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Button CloseBtn;

    public void SetActive(bool isActive)
    {
        RootingUI.SetActive(isActive);
    }
    protected override void Subscribe()
    {
        CloseBtn.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        SetActive(false);
    }

    public void SetText(MajorArcanaSO major)
    {
        image.sprite = major.sprite;
        text.text = $"{major.cardName}을 획득하였습니다.";
    }

    protected override void UnSubscribe()
    {
        CloseBtn.onClick.RemoveListener(OnButtonClicked);
    }
}
