using DesignPattern;
using InGameShop;
using Managers;
using System;
using System.Collections.Generic;
using TopBarUI;
using UnityEngine;

namespace Item
{
    public class ItemManager : Singleton<ItemManager>
    {
        private ItemEffectHandler _effectHandler;
        private List<InventorySlot> _inventorySlots = new();
        
        private void Awake()
        {
            _effectHandler = new ItemEffectHandler(Manager.turnManager.GetPlayerController());
            _instance = this;
        }

        /// <summary>
        /// 상단 바의 인벤토리 슬롯의 버튼의 lock 여부를 설정할 수 있습니다.
        /// lock을 활성화하면 인벤토리 슬롯 버튼이 비활성화되어 아이템을 사용할 수 없습니다.
        /// </summary>
        /// <param name="isLock"></param>
        public void SetInventorySlotState(bool isLock)
        {
            foreach (InventorySlot slot in _inventorySlots)
            {
                slot.SetLock(isLock);
            }
        }

        public void EnrollInventorySlot(InventorySlot slot)
        {
            if(!_inventorySlots.Contains(slot))
                _inventorySlots.Add(slot);
        }

        public void UseItem(string itemID)
        {
            InventoryItem item = Manager.Data.GameItemDB.GetItemById(itemID) as InventoryItem;
            
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
                    break;
                }
                
            }
        }
    }
}