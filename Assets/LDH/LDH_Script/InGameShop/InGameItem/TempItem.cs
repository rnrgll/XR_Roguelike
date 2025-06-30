using System;
using UnityEngine;

namespace InGameShop
{
    [Serializable]
    public class TempItem
    {
        public string id;
        public string name;
        public string description;
        public int price;
        public Sprite image;
        public float weight;
    }
}