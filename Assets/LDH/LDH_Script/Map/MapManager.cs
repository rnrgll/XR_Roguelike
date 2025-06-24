using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : Singleton<MapManager>
    {
        public MapGenerator mapGenerator = new();
        public MapConfig config; //맵 config data
        public MapData CurrentMap { get; private set; }
        
        public void Awake()
        {
            SingletonInit();
        }
        public void Start()
        {
            LoadMapConfig();
        }
        
        public void LoadMapConfig()
        {
            config = Resources.Load<MapConfig>("ScriptableObject/MapConfiguration");
        }

        public void GenerateMap()
        {
            //맵 생성
            CurrentMap = mapGenerator.GenerateMap(config);
        }
        
    }
}