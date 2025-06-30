using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GoldDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _goldText;

        private void OnEnable()
        {
            GameStateManager.Instance.OnGoldChanged.AddListener(UpdateUI);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.OnGoldChanged.RemoveListener(UpdateUI);
        }

        private void UpdateUI(int gold)
        {
            _goldText.text = gold.ToString();
        }
        
    }
}