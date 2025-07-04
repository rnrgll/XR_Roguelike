using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class PurchaseButton : MonoBehaviour
    {
        [SerializeField] private GameObject _goldAlarmPopUp;
        [SerializeField] private FullSlotAlarmPanel _fullSlotAlarmPopUp;
        private string _itemId;
        private ButtonCondition _condition;
        
        private void Awake() => Init();
        
        private void Init()
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

        }

        public void SetButton(int slotIndex, Item item)
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

            _itemId = item.id;
            
            if (GameStateManager.Instance.Gold < item.price)
            {
                _condition.SetButtonState(ButtonState.Deactive, () => _goldAlarmPopUp.SetActive(true) );
            }
            else
            {
                _condition.SetButtonState(ButtonState.Active, ()=>
                {
                    Purchase(slotIndex, (item is InventoryItem? ItemType.Item: ItemType.Card), _itemId);
                    
                });
            }
            
        }

        public void Purchase(int slotIndex, ItemType itemType, string itemID)
        {
            if (itemType == ItemType.Item &&  Manager.GameState.CurrentItemCount == 3)
            {
                _fullSlotAlarmPopUp.Show(this);
            }
            else
            {
                ShopManager.Instance.Purchase(slotIndex, itemID);
            }
        }
        
        
        
        
        
    }
}