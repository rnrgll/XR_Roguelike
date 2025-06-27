using Managers;
using System;
using UnityEngine;

namespace InGameShop
{
    public class ShopTest : MonoBehaviour
    {
        private void Awake()
        {
            Manager.Map.GenerateMap();
            Manager.Map.HideMap();
        }
    }
}