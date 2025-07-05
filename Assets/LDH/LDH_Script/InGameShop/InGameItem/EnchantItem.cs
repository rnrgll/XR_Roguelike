using CardEnum;
using Item;
using UnityEngine;

namespace InGameShop
{
    [CreateAssetMenu(menuName = "GameItem/CardItem")]
    public class EnchantItem : GameItem
    {
        [Header("Enchant Type")]
        public CardEnchant enchantType;

        public CardEnchantSO enchantSo;
        public Sprite enchantSprite;
        
        [Header("Target Card Num")]
        [Range(1, 14)] public int minTargetNum = 1;
        [Range(1, 14)] public int maxTargetNum = 14;


        private MinorSuit _suit;
        private int _cardNum;
        public MinorSuit Suit => _suit;
        public int CardNum => _cardNum;

        public void SetData(MinorSuit suit, int cardNum)
        {
            _suit = suit;
            _cardNum = cardNum;
        }
    
    }
}