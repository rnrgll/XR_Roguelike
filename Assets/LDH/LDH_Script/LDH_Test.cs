using Managers;
using System;
using System.Collections.Generic;
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
            //if(Manager.Map.MapCanvas.gameObject.activeSelf)
                Manager.Map.HideMap();
            //else
                Manager.Map.ShowMap();
        }

        public void GameStart()
        {
            Manager.Map.GenerateMap();
            Manager.UI.TopBarUI.SetActive(true);
            Manager.GameState.Init();
            Manager.GameState.AddGold(1000);
          
        }

        public void GetItems()
        {
            List<string> items = Manager.Data.ItemDB.PickUniqeItemRandom(1);
            Manager.GameState.AddItem(items[0]);
        }
    }
}