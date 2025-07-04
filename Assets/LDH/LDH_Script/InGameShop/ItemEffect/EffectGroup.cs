using System.Collections.Generic;

namespace Item
{
    [System.Serializable]
    public class EffectGroup
    {
        public float chance;                    // 이 그룹이 발동될 확률
        public List<ItemEffect> effects;  
    }
}