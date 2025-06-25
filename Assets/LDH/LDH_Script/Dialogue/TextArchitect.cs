using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CustomUtility.UI
{
    public class TextArchitect : MonoBehaviour
    {
        private TextMeshProUGUI tmpro_ui;
        private TextMeshPro tmpro_world;
        public TMP_Text tmpro => (tmpro_ui != null) ? tmpro_ui : tmpro_world;

        public string currentText => tmpro.text;
        public string targetText { get; private set; } = "";
        public string preText { get; private set; } = "";
        private int preTextLength = 0;

        public string fullTargetText => preText + targetText;
    }

}