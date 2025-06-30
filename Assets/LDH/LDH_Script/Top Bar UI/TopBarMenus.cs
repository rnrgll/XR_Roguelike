using Managers;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarMenus : MonoBehaviour
{
    [SerializeField] private Button _mapButton;
    [SerializeField] private Button _cardDeckButton;
    [SerializeField] private Button _settingButton;
    
    // Start is called before the first frame update
    void Start()
    {
        _mapButton.onClick.AddListener(ShowMap);
    }

    private void OnDestroy()
    {
        _mapButton.onClick.RemoveAllListeners();
    }

    private void ShowMap()
    {
        Manager.Map.ShowMap(ShowType.View);
    }
}
