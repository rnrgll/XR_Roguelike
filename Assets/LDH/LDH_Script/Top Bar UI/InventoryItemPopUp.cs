using Managers;
using TMPro;
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
            var item = Manager.Data.ItemDB.GetItemById(itemID);
            _itemImage.sprite = item.image;
            _itemDescription.text = $"{item.name}\n{item.description}";
        }
        
        
    }
}