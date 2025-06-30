using DesignPattern;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InGameShop
{
    public class ShopModel
    {
        public List<ObservableProperty<int>> shopSlots = new();

        public ShopModel()
        {
            shopSlots.Clear();
            for(int i=0; i<4; i++)
                shopSlots.Add(new ObservableProperty<int>(-1));
        }
        
        
        public void SetItems(List<int> newItemsID)
        {
            for (int i = 0; i < 4; i++)
            {
                shopSlots[i].Value = newItemsID[i];
            }
        }
        
        public int GetItemID(int slotIndex)
        {
            if (shopSlots.Count == 4)
                return shopSlots[slotIndex].Value;
            
            else
                return -1;
        }
        
    }
}