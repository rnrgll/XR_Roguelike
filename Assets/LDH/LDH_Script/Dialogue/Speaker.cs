using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class SpeakerPortrait
    {
        public string key;
        public Sprite sprite;
    }
    [CreateAssetMenu(menuName = "Dialogue/Speaker", order = 0)]
    public class Speaker : ScriptableObject
    {
        public string speakerName;
        public Color32 textColor;
        public Color32 textBoxColor;
        public List<SpeakerPortrait> portraits= new();

        public Sprite GetSprite(string key)
        {
            return portraits.FirstOrDefault(p => p.key.ToLower() == key.ToLower()).sprite;
        }
    }
}