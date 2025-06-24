using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LDH.LDH_Script
{
    public class LDH_Test : MonoBehaviour
    {
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Manager.Map.GenerateMap);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}