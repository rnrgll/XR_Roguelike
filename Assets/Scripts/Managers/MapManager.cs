using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;
using Map;
using System.Runtime.CompilerServices;

public class MapManager : Singleton<MapManager>
{
    public MapGenerator mapGenerator;
    public MapConfig config; //맵 config data
    public MapData CurrentMap { get; private set; }
    [field: SerializeField] public MapView View { get; private set; }
    [field: SerializeField]  public MapTracker Tracker { get; private set; }
    [field: SerializeField] public Canvas MapCanvas { get; private set; }
    
    public void Awake()
    {
        _instance = this;
    }
    public void GenerateMap()
    {
        //맵 생성
        CurrentMap = mapGenerator.GenerateMap(config);
        ShowMap();
    }

    public void ShowMap()
    {
        if (CurrentMap == null) return;
        
        MapCanvas.gameObject.SetActive(true);
        if (View.Map == null || View.Map != CurrentMap)
        {
            View.ShowMap(CurrentMap);
        }
        
    }

    public void HideMap()
    {
        if(CurrentMap == null) return;

        MapCanvas.gameObject.SetActive(false);
    }
    
}
