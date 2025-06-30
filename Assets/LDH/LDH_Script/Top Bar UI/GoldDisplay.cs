using System;
using TMPro;
using UnityEngine;

namespace LDH.LDH_Script.Top_Bar_UI
{
    public class GoldDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _goldText;

        private void OnEnable()
        {
            GameStateManager.Instance.OnGetGold.AddListener(UpdateUI);
        }

        private void OnDisable()
        {
            GameStateManager.Instance.OnGetGold.RemoveListener(UpdateUI);
        }

        private void UpdateUI(int gold)
        {
            _goldText.text = gold.ToString();
        }
        
    }
}