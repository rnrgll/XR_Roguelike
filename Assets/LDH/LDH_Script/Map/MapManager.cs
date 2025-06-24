using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : Singleton<MapManager>
    {
        public MapConfig config; //맵 config data
        public List<List<Node>> CurrentMap { get; private set; }
        
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
            CurrentMap = MapGenerator.GenerateMap(config);
        }
        
    }
}