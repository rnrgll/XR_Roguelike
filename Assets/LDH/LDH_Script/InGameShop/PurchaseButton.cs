using Item;
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
        private ButtonCondition _condition;
        
        private void Awake() => Init();
        
        private void Init()
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

        }

        public void SetButton(int slotIndex, GameItem item)
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

            
            if (GameStateManager.Instance.Gold < item.price)
            {
                _condition.SetButtonState(ButtonActiveState.Deactive, () => _goldAlarmPopUp.SetActive(true) );
            }
            else
            {
                _condition.SetButtonState(ButtonActiveState.Active, ()=>
                {
                    Purchase(slotIndex, (item is InventoryItem? ItemType.Item: ItemType.Card));
                    
                });
            }
            
        }

        public void Purchase(int slotIndex, ItemType itemType)
        {
            if (itemType == ItemType.Item &&  Manager.GameState.CurrentItemCount == 3)
            {
                _fullSlotAlarmPopUp.Show(this);
            }
            else
            {
                ShopManager.Instance.Purchase(slotIndex);
            }
        }
        
        
        
        
        
    }
}