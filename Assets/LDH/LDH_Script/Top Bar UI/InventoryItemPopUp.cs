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

        public void SetData(string itemID)
        {
            var item = Manager.Data.GameItemDB.GetItemById(itemID);
            _itemImage.sprite = item.sprite;
            _itemDescription.text = $"{item.name}\n{item.description}";
        }
        
        
    }
}