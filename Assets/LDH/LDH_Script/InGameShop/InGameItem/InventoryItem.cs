using Item;
using System.Collections.Generic;
using UnityEngine;

namespace InGameShop
{
    [CreateAssetMenu(menuName = "GameItem/InventoyItem")]
    public class InventoryItem : GameItem
    {
        public InventoryItemType type;
        public bool isPotion;
        public List<EffectGroup> effectGroups;
    }
}