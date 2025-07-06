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
            List<GameItem> enchantItems = Manager.Data.GameItemDB.PickUniqeItemRandomByType(Value, ItemType.Card);
            
            foreach (EnchantItem enchantItem in enchantItems)
            {
                Manager.GameState.AddCardItem(enchantItem);
            }
        }
    }
}