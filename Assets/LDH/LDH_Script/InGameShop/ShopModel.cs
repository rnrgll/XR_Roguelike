using DesignPattern;
using Item;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InGameShop
{
    public class ShopModel
    {
        public List<ObservableProperty<GameItem>> shopSlots = new();

        public ShopModel()
        {
            shopSlots.Clear();
            for(int i=0; i<4; i++)
                shopSlots.Add(new ObservableProperty<GameItem>());
        }
            
        
        public void SetItems(List<GameItem> newItems)
        {
            for (int i = 0; i < 4; i++)
            {
                shopSlots[i].Value = newItems[i];
            }
        }
        
        public GameItem GetItem(int slotIndex)
        {
            if (shopSlots.Count == 4)
                return shopSlots[slotIndex].Value;

            else
                return null;
        }
        
    }
}