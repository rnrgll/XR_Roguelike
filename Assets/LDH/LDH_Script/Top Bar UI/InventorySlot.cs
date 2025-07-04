using InGameShop;
using Item;
using Managers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace TopBarUI
{
    public class InventorySlot : MonoBehaviour
    {
        private Toggle _slotToggle;
        [SerializeField] private Image _itemImage;
        
        public int slotIndex;
        public string itemId;
        
        private void Awake()
        {
            _slotToggle = GetComponent<Toggle>();
            
        }

        private void OnEnable()
        {
            Manager.GameState.OnItemChanged.AddListener(UpdateSlot);
            _slotToggle.onValueChanged.AddListener(OnSlotToggleChanged);
        }

        private void OnDisable()
        {
            Manager.GameState.OnItemChanged.RemoveListener(UpdateSlot);
            _slotToggle.onValueChanged.RemoveListener(OnSlotToggleChanged);
        }


        private void UpdateSlot(string itemId)
        {
            this.itemId = Manager.GameState.ItemInventory[slotIndex];
            if (string.IsNullOrEmpty(this.itemId))
            {
                _itemImage.sprite = null;
            }
            else
            {
                GameItem item = Manager.Data.ItemDB.GetItemById(this.itemId);
                _itemImage.sprite = item.sprite;
            }
            
        }

        private void OnSlotToggleChanged(bool isON)
        {
            if (isON)
            {
                if (string.IsNullOrEmpty(itemId)) return;
                SendData();
            }
            Manager.UI.SetUIActive(GlobalUI.Item, isON);
        }
        
        private void SendData()
        {
            if (string.IsNullOrEmpty(itemId)) return;
            Manager.UI.ItemUI.GetComponent<InventoryItemPopUp>().SetData(itemId);
        }
        
    }
}