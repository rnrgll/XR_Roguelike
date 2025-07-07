using InGameShop;
using Item;
using Managers;
using System;
using System.Collections.Generic;

namespace Event
{
    public class ObtainCardEffect : SubEffect
    {
        public ObtainCardEffect(int value) : base(SubEffectType.ObtainEnhancedCard, value)
        { }

        public override void ApplyEffect(Action onComplete)
        {
            base.ApplyEffect(null);
            List<GameItem> enchantItems = Manager.Data.GameItemDB.PickUniqeItemRandomByType(Value, ItemType.Card);

            foreach (EnchantItem enchantItem in enchantItems)
            {
                Manager.GameState.AddCardItem(enchantItem);
            }

            onComplete?.Invoke();
        }
    }
}