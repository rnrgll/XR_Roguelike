using UnityEngine;

namespace Dialogue
{
    public class DialogueLine
    {
        public int index;
        public string speakerName;
        public string position;
        public string portraitKey;
        public string dialogueText;

        public Speaker GetSpeaker()
        {
            return Resources.Load<Speaker>($"Speaker/{speakerName}");
        }
    }
}