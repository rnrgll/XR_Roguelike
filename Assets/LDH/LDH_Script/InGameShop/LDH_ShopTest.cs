using Managers;
using System;
using UnityEngine;

namespace InGameShop
{
    public class LDH_ShopTest : MonoBehaviour
    {
        private void Awake()
        {
            Manager.Map.GenerateMap();
            Manager.Map.HideMap();
        }
    }
}