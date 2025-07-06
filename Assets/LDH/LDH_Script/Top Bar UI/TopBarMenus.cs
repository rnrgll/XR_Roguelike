using Managers;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TopBarUI
{

    public class TopBarMenus : MonoBehaviour
    {
        [SerializeField] private Toggle _mapToggle;
        [SerializeField] private Toggle _cardDeckToggle;
        [SerializeField] private Toggle _settingToggle;
        public Toggle CardDeckToggle => _cardDeckToggle;
        public Action<bool> OnCardDeckToggleClicked;
        
        // Start is called before the first frame update
        void Start()
        {
            _mapToggle.onValueChanged.AddListener(OnMapToggleChanged);
            _cardDeckToggle.onValueChanged.AddListener(OnDeckToggleChanged);
            _settingToggle.onValueChanged.AddListener(OnSettingToggleChanged);

        }

        private void OnDestroy()
        {
            _mapToggle.onValueChanged.RemoveAllListeners();
            _cardDeckToggle.onValueChanged.RemoveAllListeners();
            _settingToggle.onValueChanged.RemoveAllListeners();
        }


        private void OnMapToggleChanged(bool isOn)
        {
            if (Manager.Map.MapViewLock)
                return;
            Manager.UI.SetUIActive(GlobalUI.Map, isOn);
        }

        private void OnDeckToggleChanged(bool isOn)
        {
            Manager.UI.SetUIActive(GlobalUI.Deck, isOn);
            OnCardDeckToggleClicked?.Invoke(isOn);
        }

        private void OnSettingToggleChanged(bool isOn)
        {
            //Manager.UI.SetUIActive(GlobalUI.Setting, isOn);
        
        }

    }
}
