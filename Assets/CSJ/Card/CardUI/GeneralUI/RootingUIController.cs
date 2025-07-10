using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RootingUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image image;
    [SerializeField] Button CloseBtn;

    private void Start()
    {
        CloseBtn.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        CloseBtn.onClick.RemoveListener(OnButtonClicked);
    }
    

    private void OnButtonClicked()
    {
        Manager.UI.SetUIActive(GlobalUI.Rooting, false);
        Manager.UI.ShowSelectableMap();
    }

    public void SetText(MajorArcanaSO major)
    {
        image.sprite = major.sprite;
        text.text = $"{major.cardName}, 200 골드를 획득하였습니다.";
    }
    
}
