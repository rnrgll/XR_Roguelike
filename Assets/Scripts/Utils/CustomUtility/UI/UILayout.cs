using UnityEngine;

namespace CustomUtility.UI
{
    public static class UILayout
    {
        /// <summary>
        /// 부모 RectTransform에 맞춰 스트레치하며 여백 설정
        /// Vector4(left, top, right, bottom)
        /// </summary>
        public static void Stretch(RectTransform tr, Vector4 margin =default)
        {
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(margin.x, margin.w); // left, bottom
            
            tr.offsetMax = new Vector2(-margin.z, -margin.y); // -right, -top
        }

    }
}