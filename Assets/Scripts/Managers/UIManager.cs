using DesignPattern;
using System;
using UnityEngine;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        #region Global UI Object

        public GameObject TopBarUI { get; private set; }
        public GameObject MapUI { get; private set; }
        

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
        }
    }
}