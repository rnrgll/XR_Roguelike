using Managers;
using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{

    public class TopBarMenus : MonoBehaviour
    {
        [SerializeField] private Button _mapButton;
        [SerializeField] private Button _cardDeckButton;
        [SerializeField] private Button _settingButton;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _mapButton.onClick.AddListener(ShowMap);
            _settingButton.onClick.AddListener(ShowSetting);
            _cardDeckButton.onClick.AddListener(ShowDeck);
        }

        private void OnDestroy()
        {
            _mapButton.onClick.RemoveAllListeners();
            _settingButton.onClick.RemoveAllListeners();
            _cardDeckButton.onClick.RemoveAllListeners();
            
        }

        private void ShowMap()
        {
            Manager.UI.ToggleUI(GlobalUI.Map);
        }

        private void ShowDeck()
        {
            Manager.UI.ToggleUI(GlobalUI.Deck);
        }

        private void ShowSetting()
        {
            //todo : 게임 진입시 인게임 일시정지 필요
            SceneManager.LoadSceneAsync("Option", LoadSceneMode.Additive);
        }
    }
}
