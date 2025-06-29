using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class PurchaseButton : MonoBehaviour
    {
        [SerializeField] private GameObject _alarmPopUp;
        
        private int _itemIndex;
        private ButtonCondition _condition;
        
        private void Awake() => Init();
        
        private void Init()
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();

        }

        public void SetButton(int slotIndex, TempItemClass item)
        {
            if (_condition == null)
                _condition = GetComponent<ButtonCondition>();
            
            if (GameStateManager.Instance.Gold < item.price)
            {
                _condition.SetButtonState(ButtonState.Deactive, () => _alarmPopUp.SetActive(true) );
            }
            else
            {
                _condition.SetButtonState(ButtonState.Active, ()=>
                {
                    Purchase(slotIndex, _itemIndex);
                    
                });
            }
            
        }

        private void Purchase(int slotIndex, int itemID)
        {
            ShopManager.Instance.Purchase(slotIndex, itemID);
        }
        
        
        
        
        
    }
}