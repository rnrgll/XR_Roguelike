using InGameShop;
using Item;
using Managers;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Event
{
    public class ObtainItemEffect : SubEffect
    {
        private bool isPotion;
        public ObtainItemEffect(int value, bool isPotion) : base(SubEffectType.ObtainItem, value)
        {
            this.isPotion = isPotion;
        }

        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(null);
            
            //인벤토리 포화상태 체크
            int emptySlotCnt = Manager.GameState.MaxItemInventorySize - Manager.GameState.CurrentItemCount;
            Debug.Log($"MaxSize: {Manager.GameState.MaxItemInventorySize}, CurrentCount: {Manager.GameState.CurrentItemCount}, Request: {Value}, emptyslot : {emptySlotCnt}");

            if (emptySlotCnt < Value)
            {
                //인벤토리 비우기
                Manager.UI.ItemRemoveUI.InitPanel(Value-emptySlotCnt,
                    () =>
                    {
                        GetItems();
                        onComplete?.Invoke();
                    });
                Manager.UI.SetUIActive(GlobalUI.ItemRemove,true);
            }
            else
            {
                GetItems();
                onComplete?.Invoke();
            }

        }

        public void GetItems()
        {
            List<GameItem> newItems = new();
           
            if (isPotion)
                //potion 타입만 뽑기
                newItems = Manager.Data.GameItemDB.PickRandomPotionItems(Value);
            else
                //아이템 Value개 뽑기
                newItems = Manager.Data.GameItemDB.PickUniqeItemRandomByType(Value, ItemType.Item);
            
            //인벤토리에 추가
            for (int i = 0; i < newItems.Count; i++)
            {
                Manager.GameState.AddItem(newItems[i].id);
            }

        }
    }
}