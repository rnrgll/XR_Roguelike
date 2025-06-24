using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class MapManager : Singleton<MapManager>
{
    public MapGenerator mapGenerator;
    public MapConfig config; //맵 config data
    public MapData CurrentMap { get; private set; }
    public MapView View;
    
    public void Awake()
    {
        SingletonInit();
    }
    public void GenerateMap()
    {
        //맵 생성
        CurrentMap = mapGenerator.GenerateMap(config);
        View.ShowMap(CurrentMap);
    }
    
}
