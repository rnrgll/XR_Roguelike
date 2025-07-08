using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RootingUIController : UIRequire
{
    [SerializeField] GameObject RootingUI;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Button CloseBtn;
    
    
    //todo : 수정 필요(도현)
    [SerializeField] private Button turnEndButton;
    [SerializeField] private GameObject _majorArcanaCanvas;
    
    
    public void SetActive(bool isActive)
    {
        Debug.Log(_majorArcanaCanvas==null);
        if(isActive)
            _majorArcanaCanvas?.SetActive(false);
        RootingUI.SetActive(isActive);
    }
    protected override void Subscribe()
    {
        CloseBtn.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        SetActive(false);
        Manager.UI.ShowSelectableMap();
        
    }

    public void SetText(MajorArcanaSO major)
    {
        image.sprite = major.sprite;
        text.text = $"{major.cardName}, 200 골드를 획득하였습니다.";
    }

    protected override void UnSubscribe()
    {
        CloseBtn.onClick.RemoveListener(OnButtonClicked);
    }
}
