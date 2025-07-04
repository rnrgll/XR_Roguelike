using System;
using UnityEngine;

namespace Item
{
    public class GameItem : ScriptableObject
    {
        public ShopType shopType;
        public InGameShop.ItemType itemType;
        public string id;
        public string name;
        public string description;
        public int price;
        public Sprite sprite;
        public float weight;
    }
}