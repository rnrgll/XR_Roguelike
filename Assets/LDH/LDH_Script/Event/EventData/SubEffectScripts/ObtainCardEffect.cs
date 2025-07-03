using InGameShop;
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
            List<string> cardItems = Manager.Data.ItemDB.PickUniqeItemRandomByType(Value, ItemType.Card);

            foreach (string cardItem in cardItems)
            {
                Manager.GameState.AddCardItem(cardItem);
            }
        }
    }
}