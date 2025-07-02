using UnityEngine;

namespace InGameShop
{
    public class FullSlotAlarmPanel : MonoBehaviour
    {
        private PurchaseButton _purchaseButton;

        public void Show(PurchaseButton purchaseButton)
        {
            
            gameObject.SetActive(true);
            
        }
    }
}