using Item;
using UnityEngine;

namespace InGameShop
{
    [CreateAssetMenu(menuName = "GameItem/CardItem")]
    public class CardItem : GameItem
    {
        public string cardName;
    }
}