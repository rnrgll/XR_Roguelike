using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class PurchaseButton : MonoBehaviour
    {
        [SerializeField] private GameObject _alarmPopUp;
        
        private string _itemId;
        private ButtonCondition _condition;
        
        private void Awake() => Init();
        
        private void Init()
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

        }

        public void SetButton(int slotIndex, TempItem item)
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

            _itemId = item.id;
            
            if (GameStateManager.Instance.Gold < item.price)
            {
                _condition.SetButtonState(ButtonState.Deactive, () => _alarmPopUp.SetActive(true) );
            }
            else
            {
                _condition.SetButtonState(ButtonState.Active, ()=>
                {
                    Purchase(slotIndex, _itemId);
                    
                });
            }
            
        }

        private void Purchase(int slotIndex, string itemID)
        {
            ShopManager.Instance.Purchase(slotIndex, itemID);
        }
        
        
        
        
        
    }
}