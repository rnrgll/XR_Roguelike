using System;
using System.Collections.Generic;
using UnityEngine;

namespace LDH.LDH_Script
{
    public class ItemTest : MonoBehaviour
    {
        //201~215
        public List<bool> itemAddList;

        public void AddItem()
        {
            for (int i = 0; i < itemAddList.Count; i++)
            {
                if (itemAddList[i])
                {
                    Debug.Log("아이템 추가");
                    GameStateManager.Instance.AddItem((i+201).ToString());
                }
                if(GameStateManager.Instance.ItemInventory.Count==3)
                    break;
            }
            
        }
    }
}