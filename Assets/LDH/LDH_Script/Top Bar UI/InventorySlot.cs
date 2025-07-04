using InGameShop;
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
        private Button _slotButton;
        [SerializeField] private Image _itemImage;
        
        public int slotIndex;
        public string itemId;
        
        private void Awake()
        {
            _slotButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            Manager.GameState.OnItemChanged.AddListener(UpdateSlot);
            _slotButton.onClick.AddListener(ShowItem);
        }

        private void OnDisable()
        {
            Manager.GameState.OnItemChanged.RemoveListener(UpdateSlot);
            _slotButton.onClick.RemoveListener(ShowItem);
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
                Item item = Manager.Data.ItemDB.GetItemById(this.itemId);
                _itemImage.sprite = item.sprite;
            }
            
        }

        private void ShowItem()
        {
            if (string.IsNullOrEmpty(itemId)) return;
            SendData();
            Manager.UI.ToggleUI(ToggleUI.Item);
        }
        
        private void SendData()
        {
            if (string.IsNullOrEmpty(itemId)) return;
            Manager.UI.ItemUI.GetComponent<InventoryItemPopUp>().SetData(itemId);
        }
        
    }
}