using DesignPattern;
using Item;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InGameShop
{
    public class ShopModel
    {
        public List<ObservableProperty<string>> shopSlots = new();

        public ShopModel()
        {
            shopSlots.Clear();
            for(int i=0; i<4; i++)
                shopSlots.Add(new ObservableProperty<string>(String.Empty));
        }
            
        
        public void SetItems(List<GameItem> newItems)
        {
            for (int i = 0; i < 4; i++)
            {
                shopSlots[i].Value = newItems[i].id;
            }
        }
        
        public string GetItemID(int slotIndex)
        {
            if (shopSlots.Count == 4)
                return shopSlots[slotIndex].Value;
            
            else
                return string.Empty;
        }
        
    }
}