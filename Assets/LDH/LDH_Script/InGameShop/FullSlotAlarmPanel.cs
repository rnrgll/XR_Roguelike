using Managers;
using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace InGameShop
{
    public class FullSlotAlarmPanel : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        private Button _purchaseButton;
        private void OnDisable()
        {
            _selectButton.onClick.RemoveAllListeners();
        }

        public void Show(PurchaseButton purchaseButton)
        {
            _purchaseButton = purchaseButton.GetComponent<Button>();
            
            _selectButton.onClick.AddListener(PurchaseAfterRemove);
            
            _selectButton.onClick.AddListener(()=>{gameObject.SetActive(false);});
            
            gameObject.SetActive(true);
        }

        private void PurchaseAfterRemove()
        {
            Manager.UI.ItemRemoveUI.GetComponent<RemoveItemPanel>().InitPanel(1, 
                () =>
                {
                    _purchaseButton.onClick?.Invoke();
                    _purchaseButton = null;
                });
            Manager.UI.SetUIActive(GlobalUI.ItemRemove,true);
        }
    }
}