using Event.Dialogue;
using Item;
using Managers;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InGameShop
{
    public class ShopManager : MonoBehaviour
    {
        //로컬 싱글톤
        public static ShopManager Instance { get; private set; }
        
        [SerializeField] private List<ItemButton> itemButtons;
        private ShopModel model;
        private ShopController controller;
        
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            
            UnBind();
        }

        private void Start()
        {
            //모델, 컨트롤러 생성
            model = new();
            controller = new(model);
            
            //버튼과 모델 바인딩
            for (int i = 0; i < 4; i++)
            {
                model.shopSlots[i].Subscribe(itemButtons[i].OnItemUpdated);
            }

            //아이템 리롤
            Reroll();
            
            
            //상점 진입시 다이얼로그
            if(Manager.Map.CurrentMap.CurrentNode.y == 1)
                Manager.Dialogue.PlayDialogue(DialogueType.FirstInGameShop);
            else
            {
                Debug.Log(Manager.Map.CurrentMap.CurrentNode.y );
                Manager.Dialogue.PlayDialogue(DialogueType.InGameShop);
            }
                
        }
        

        //버튼과 모델 바인딩 해제
        public void UnBind()
        {
            foreach (var property in model.shopSlots)
            {
                property.UnsbscribeAll();
            }
        }
        
        public void Purchase(int slotIndex)
        {
            var button = itemButtons.Find(button => button.slotIndex == slotIndex);
            
            button?.ReturnToOrigin();
            button?.transform.parent.gameObject.SetActive(false);
            
            controller.Purchase(model.shopSlots[slotIndex].Value);
        }

        public void Reroll()
        {
            foreach (var button in itemButtons)
            {
                button.transform.parent.gameObject.SetActive(true);
            }
            
            controller.Reroll();
        }
        
        public GameItem GetItem(int slotIndex) => model.GetItem(slotIndex);
        
        //나가기 
        public void ExitShop()
        {
            Manager.Map.ShowMap();
        }
    }
}