using InGameShop;
using Managers;
using System.Collections.Generic;

namespace Event
{
    public class ObtainItemEffect : SubEffect
    {
        private bool isPotion;
        public ObtainItemEffect(int value, bool isPotion) : base(SubEffectType.ObtainItem, value)
        {
            this.isPotion = isPotion;
        }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            //todo: potion 타입만 뽑아 올 수 있게 수정하기
            
            //아이템 Value개 뽑기
            List<string> newItems = Manager.Data.ItemDB.PickUniqeItemRandomByType(Value, ItemType.Item);
            
            //인벤토리에 추가
            for (int i = 0; i < newItems.Count; i++)
            {
                Manager.GameState.AddItem(newItems[i]);
            }


        }
    }
}