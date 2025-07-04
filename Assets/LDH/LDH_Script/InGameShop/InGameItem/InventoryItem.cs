using Item;
using UnityEngine;

namespace InGameShop
{
    [CreateAssetMenu(menuName = "GameItem/InventoyItem")]
    public class InventoryItem : GameItem
    {
        public int shopId;
        public int itemId;
        
        public ItemType type;
        public int price;

        public ItemEffect baseEffect;
        public ItemEffect extraEffect;

        public string condition; // OnSuccessOnly, RandomPick 등
    }
}