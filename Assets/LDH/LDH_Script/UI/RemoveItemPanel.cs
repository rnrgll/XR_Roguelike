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
            
            for(int i =0; i<Manager.GameState.ItemInventory.Count; i++)
            {
                UpdateSlot(i, Manager.GameState.ItemInventory[i]);
                slots[i].onClick.AddListener(() => OnSlotClicked(i));
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
            slots[slotIndex].GetComponentInChildren<Image>().sprite = item.sprite;
        }

        private void OnSlotClicked(int slotIndex)
        {
            Manager.GameState.RemoveItem(Manager.GameState.ItemInventory[slotIndex]);
            Manager.UI.SetUIActive(GlobalUI.ItemRemove, false);
            
            _onItemRemoved?.Invoke();
            _onItemRemoved = null; // 초기화
        }
        
        
        
    }
}