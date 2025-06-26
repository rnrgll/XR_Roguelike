using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace LDH.LDH_Script
{
    public class LDH_Test : MonoBehaviour
    {
        
        public void GenerateMap()
        {
            Manager.Map.GenerateMap();
            Manager.Map.ShowMap();
        }

        public void ShowHideMap()
        {
            if(Manager.Map.MapCanvas.gameObject.activeSelf)
                Manager.Map.HideMap();
            else
                Manager.Map.ShowMap();
        }
    }
}