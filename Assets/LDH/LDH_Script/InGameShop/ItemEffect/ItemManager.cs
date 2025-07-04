using DesignPattern;
using InGameShop;
using Managers;
using System;
using UnityEngine;

namespace Item
{
    public class ItemManager : Singleton<ItemManager>
    {
        
        private ItemEffectHandler _effectHandler;
        private void Awake()
        {
            _effectHandler = new ItemEffectHandler(Manager.turnManager.GetPlayerController());
            
            SingletonInit();
        }

        public void UseItem(string itemID)
        {
            InventoryItem item = Manager.Data.ItemDB.GetItemById(itemID) as InventoryItem;
            
            //아이템 인벤토리에서 제거
            Manager.GameState.RemoveItem(itemID);
            
            
            if (item.effectGroups == null || item.effectGroups.Count == 0) return;

            float roll = Manager.randomManager.RandFloat(0, 1);
            float cumulateChance = 0f;
            
            
            foreach (var group in item.effectGroups)
            {
                cumulateChance += group.chance;
                if (roll <= cumulateChance)
                {
                    Debug.Log($"[아이템] : 효과 선택 완료! / 선택 효과 확률 : {group.chance} / roll : {roll}");
                    foreach (var effect in group.effects)
                    {
                        _effectHandler.ApplyEffect(effect);
                    }   
                }
                
            }
            
        }
    }
}