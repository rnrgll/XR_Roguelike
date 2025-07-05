using InGameShop;
using Item;
using Managers;
using System.Collections.Generic;

namespace Event
{
    public class ObtainCardEffect : SubEffect
    {
        public ObtainCardEffect(int value) : base(SubEffectType.ObtainEnhancedCard, value)
        { }

        public override void ApplyEffect()
        {
            base.ApplyEffect();
            List<GameItem> cardItems = Manager.Data.ItemDB.PickUniqeItemRandomByType(Value, ItemType.Card);

            //todo:나중에 할꺼임
            // foreach (string cardItem in cardItems)
            // {
            //     Manager.GameState.AddCardItem(cardItem);
            // }
        }
    }
}