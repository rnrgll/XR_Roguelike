using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{ 
    public class CloseMenuButton : MonoBehaviour
    {
        [SerializeField] private GlobalUI _globalUIType;
        
        private Button _button;
        // Start is called before the first frame update
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _button.onClick.AddListener(CloseMenuUI);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void CloseMenuUI()
        {
            Manager.UI.ToggleUI(_globalUIType);
        }

    }
}