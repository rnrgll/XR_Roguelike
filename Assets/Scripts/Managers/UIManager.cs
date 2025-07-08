using Map;
using System;
using TopBarUI;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : DesignPattern.Singleton<UIManager>
    {
        #region Global UI Object

        public GameObject TopBarUI { get; private set; }
        public TopBarMenus topBarMenus;
        public GameObject MapUI { get; private set; }
        public GameObject DeckUI { get; private set; }
        public GameObject SettingUI { get; private set; }
        public GameObject ItemUI { get; private set; }

        public RemoveItemPanel ItemRemoveUI { get; private set; }

        public Action<bool> OnGlobalUIActive;
        

        #endregion


        private void Awake()
        {
            SingletonInit();
        }

        private void Start()
        {
            //Global UI 생성하기
            //1. container 생성
            GameObject container = new GameObject("Global UI");
            container.transform.SetParent(transform);

            //2. Global UI 프리팹 인스턴스 생성
            // 1) top bar
            var topBarPrefab = Resources.Load<GameObject>("Prefabs/@TopBarUI");
            TopBarUI = GameObject.Instantiate(topBarPrefab, container.transform);
            TopBarUI.SetActive(false);
            topBarMenus = TopBarUI.GetComponent<TopBarMenus>();

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
            
            // Setting Canvas
            var settingPrefab = Resources.Load<GameObject>("Prefabs/@Setting");
            SettingUI = Instantiate(settingPrefab, container.transform);
            SettingUI.SetActive(false);

            // 4) Item Canvas
            var itemPrefab = Resources.Load<GameObject>("Prefabs/@ItemUI");
            ItemUI = Instantiate(itemPrefab, container.transform);
            ItemUI.SetActive(false);

            // 5) Item Remove Canvas
            var itemRemovePrefab = Resources.Load<GameObject>("Prefabs/@ItemRemoveUI");
            ItemRemoveUI = Instantiate(itemRemovePrefab, container.transform).GetComponent<RemoveItemPanel>();
            ItemRemoveUI.gameObject.SetActive(false);

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
                        if (isActive)
                            Manager.Map.ShowMap(ShowType.View);
                        else
                            Manager.Map.HideMap();
                    }
                    break;
                case GlobalUI.Deck:
                    DeckUI?.SetActive(isActive);
                    break;
                case GlobalUI.Setting:
                    SettingUI?.SetActive(isActive);
                    break;
                case GlobalUI.Item:
                    ItemUI?.SetActive(isActive);
                    break;
                case GlobalUI.ItemRemove:
                    ItemRemoveUI?.gameObject.SetActive(isActive);
                    break;
            }

            bool isGloblalUIOn = MapUI.activeSelf || SettingUI.activeSelf || DeckUI.activeSelf;
            OnGlobalUIActive?.Invoke(!isGloblalUIOn);
        }

        public void ShowSelectableMap()
        {
            Manager.Map.ShowMap(ShowType.Select);
            bool isGloblalUIOn = MapUI.activeSelf || SettingUI.activeSelf || DeckUI.activeSelf;
            Debug.Log(isGloblalUIOn);
            OnGlobalUIActive?.Invoke(!isGloblalUIOn);
        }
    }
}