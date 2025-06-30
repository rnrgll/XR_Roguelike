using DesignPattern;
using InGameShop;
using System;
using UnityEngine;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {

        public ItemDataBase ItemDB;
        
        private void Awake()
        {
            SingletonInit();
        }

        private void Start()
        {
            ItemDB = new ItemDataBase();
        }
    }
}