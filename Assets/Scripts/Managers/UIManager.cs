using Map;
using System;
using UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class UIManager : DesignPattern.Singleton<UIManager>
    {
        #region Global UI Object

        public GameObject TopBarUI { get; private set; }
        public GameObject MapUI { get; private set; }
        
        public GameObject DeckUI { get; private set; }
        public GameObject ItemUI { get; private set; }
        
        private GlobalUI? _currentOpenUI = null;

        #endregion
        
        
        private void Awake()
        {
            SingletonInit();
        }

        private void Start()
        {
            
            Debug.Log("UI 생성합니다.");
            //Global UI 생성하기
            //1. container 생성
            GameObject container = new GameObject("Global UI");
            container.transform.SetParent(transform);
            
            //2. Global UI 프리팹 인스턴스 생성
            // 1) top bar
            var topBarPrefab = Resources.Load<GameObject>("Prefabs/@TopBarUI");
            TopBarUI = GameObject.Instantiate(topBarPrefab, container.transform);
            TopBarUI.SetActive(false);
            
            // 2) map canvas
            var mapPrefab = Resources.Load<GameObject>("Prefabs/@MapUI");
            MapUI = GameObject.Instantiate(mapPrefab, container.transform);
            //map manager와 연결
            Manager.Map.SetUp(MapUI.GetComponent<Canvas>());
            MapUI.SetActive(false);
              
            // 3) Deck Canvas
            var deckPrefab = Resources.Load<GameObject>("Prefabs/@DeckUI");
            DeckUI = GameObject.Instantiate(deckPrefab, container.transform);
            DeckUI.SetActive(false);
            
            // 4) Item Canvas
            var itemPrefab = Resources.Load<GameObject>("Prefabs/@ItemUI");
            ItemUI = Instantiate(itemPrefab, container.transform);
            ItemUI.SetActive(false);
        }

        public void ToggleUI(GlobalUI uiType)
        {
            if (_currentOpenUI == uiType)
            {
                // 이미 열려 있던 UI면 닫고 상태 초기화
                SetUIActive(uiType, false);
                _currentOpenUI = null;
                return;
            }

            // 다른 UI 열려있으면 닫기
            if (_currentOpenUI.HasValue)
                SetUIActive(_currentOpenUI.Value, false);

            // 새 UI 열기
            SetUIActive(uiType, true);
            _currentOpenUI = uiType;
        }
        
        public void SetUIActive(GlobalUI uiType, bool isActive)
        {
            switch (uiType)
            {
                case GlobalUI.TopBar:
                    TopBarUI?.SetActive(isActive);
                    break;
                case GlobalUI.Map:
                    if (MapUI != null)
                    {
                        if(isActive)
                            Manager.Map.ShowMap(ShowType.View);
                        else
                            Manager.Map.HideMap();
                    }
                    break;
                case GlobalUI.Deck:
                    DeckUI?.SetActive(isActive);
                    break;
                case GlobalUI.Item:
                    ItemUI?.SetActive(isActive);
                    break;
            }
        }
    }
}