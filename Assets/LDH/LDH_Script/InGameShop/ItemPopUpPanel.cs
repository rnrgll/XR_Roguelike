using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace InGameShop
{
    public class ItemPopUpPanel : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private TMP_Text _itemInfo;
        [SerializeField] private TMP_Text _itemPrice;
        [SerializeField] private PurchaseButton _purchaseButton;
        
        public void Show(int slotIndex)
        {
            gameObject.SetActive(true);
            
            string itemId = ShopManager.Instance.GetItemId(slotIndex);
            
            var item = Manager.Data.ItemDB.GetItemById(itemId);
            
            UpdateUI(item);
            _purchaseButton.SetButton( slotIndex, item);
        }
        
        public void Hide()
        {
            ClearUI();
            gameObject.SetActive(false);
        }

        private void UpdateUI(Item itemData)
        {
            _itemInfo.text = $"{itemData.name}\n{itemData.description}";

            _itemPrice.text = itemData.price.ToString();
           
            
            
        }

        private void ClearUI()
        {
            _itemInfo.text = "";
            _itemPrice.text = "";
        }
        
        
        

        
    }
}