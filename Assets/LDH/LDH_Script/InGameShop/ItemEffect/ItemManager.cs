using InGameShop;
using Managers;
using System;
using UnityEngine;

namespace Item
{
    public class ItemManager : MonoBehaviour
    {
        private ItemEffectHandler _effectHandler;
        private void Awake()
        {
            _effectHandler = new ItemEffectHandler(Manager.turnManager.GetPlayerController());
        }

        public void UseItem(InventoryItem item)
        {
            _effectHandler.ApplyEffect(item);
        }
    }
}