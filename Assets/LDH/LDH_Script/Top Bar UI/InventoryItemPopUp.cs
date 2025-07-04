using Item;
using Managers;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace TopBarUI
{
    public class InventoryItemPopUp : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _itemDescription;

        [SerializeField] private Button _useButton;
        private string currentItemId;

        private void OnDisable()
        {
            currentItemId = string.Empty;
        }

        public void SetData(string itemID)
        {
            currentItemId = itemID;
            var item = Manager.Data.ItemDB.GetItemById(itemID);
            _itemImage.sprite = item.sprite;
            _itemDescription.text = $"{item.name}\n{item.description}";
        }
        
        
        public void UseItem()
        {
            ItemManager.Instance.UseItem(currentItemId);
            Manager.UI.SetUIActive(GlobalUI.Item, false);
        }
        
    }
}