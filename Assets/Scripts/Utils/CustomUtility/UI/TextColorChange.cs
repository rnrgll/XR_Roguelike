using TMPro;
using UnityEngine;

namespace UI
{
    public class TextColorChange : MonoBehaviour
    {
        [SerializeField] private Color32 _originColor = new Color32(255,255,255,255);
        [SerializeField] private Color32 _targetColor = new (0,0,0,255);
        [SerializeField] private TMP_Text _tmpText;

        private void Awake() => Init();

        private void Init()
        {
            _tmpText = _tmpText ?? GetComponent<TMP_Text>();
        }

        public void ChangeToTargetColor()
        {
            _tmpText.color = _targetColor;
        }

        public void ReturnToOriginColor()
        {
            _tmpText.color = _originColor;
        }
    }
}