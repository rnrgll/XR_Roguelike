using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGameShop
{
    public class ShopController : MonoBehaviour
    {
        //나가기 
        public void ExitShop()
        {
            Manager.Map.ShowMap();
        }
    }
}