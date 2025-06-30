using UnityEngine;

namespace InGameShop
{
    [CreateAssetMenu(menuName = "Item/Config", order = 0)]
    public class ItemConfig : ScriptableObject
    {
        public float weight;
    }
}