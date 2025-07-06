using System;
using UnityEngine;

namespace Item
{
    public class GameItem : ScriptableObject
    {
        public ShopType shopType = ShopType.InGame;
        public InGameShop.ItemType itemType;
        
        public string id;
        public string itemName;
        [TextArea(1,3)]public string description;
        public int price;
        public Sprite sprite;
        public float weight;
    }
}