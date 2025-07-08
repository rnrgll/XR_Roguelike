using CardEnum;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class EnchantFrameColor
    {
        public static Dictionary<CardEnchant, string> cardEnchantColorMap = new()
        {
            { CardEnchant.Bonus, "#F2D56D" },
            { CardEnchant.Mult, "#00C9A7" },
            { CardEnchant.Wild, "#B5179E" },
            { CardEnchant.Glass, "#A3D9FF" },
            { CardEnchant.Steel, "#6E7B8B" },
            { CardEnchant.Gold, "#FFE452" },
            { CardEnchant.Lucky, "#90EE90" }
        };
        
        public static Color GetColor(CardEnchant enchant)
        {
            if (cardEnchantColorMap.TryGetValue(enchant, out string hex))
            {
                if (ColorUtility.TryParseHtmlString(hex, out Color color))
                    return color;
            }

            // 실패 시 디폴트로 white 반환
            Debug.LogWarning($"[CardColorUtility] 색상 없음: {enchant}");
            return Color.white;
        }
    }
}