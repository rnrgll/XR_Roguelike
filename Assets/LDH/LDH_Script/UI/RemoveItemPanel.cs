using InGameShop;
using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RemoveItemPanel : MonoBehaviour
    {
        [SerializeField] private List<Button> slots;

        private Action _onItemRemoved; //아이템 remove 후에 콜백으로 실행할 액션 (아이템 추가 등등...)
        private void OnEnable()
        {
            
            for(int i =0; i < Manager.GameState.MaxItemInventorySize; i++)
            {
                int slotIndex = i;
                if (Manager.GameState.CurrentItemCount!=3) return;
                
                UpdateSlot(i, Manager.GameState.ItemInventory[slotIndex]);
                slots[slotIndex].onClick.AddListener(() => OnSlotClicked(slotIndex));
            }
        }

        private void OnDisable()
        {
            foreach (var slot in slots)
            {
                slot.onClick.RemoveAllListeners();
            }
        }

        public void SetCallBack(Action callback)
        {
            _onItemRemoved = callback;
        }


        private void UpdateSlot(int slotIndex, string itemId)
        {
           
            TempItem item = Manager.Data.ItemDB.GetItemById(itemId);
            slots[slotIndex].transform.GetChild(0).GetComponent<Image>().sprite = item.sprite;
        }

        private void OnSlotClicked(int slotIndex)
        {
            Debug.Log($"slot index = {slotIndex}, ");
            Debug.Log($"itemid = {Manager.GameState.ItemInventory[slotIndex]}");
            Manager.GameState.RemoveItem(Manager.GameState.ItemInventory[slotIndex]);
            Manager.UI.SetUIActive(GlobalUI.ItemRemove, false);
            
            _onItemRemoved?.Invoke();
            _onItemRemoved = null; // 초기화
        }
        
        
        
    }
}