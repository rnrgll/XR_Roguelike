using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;
using Map;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

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

    public void SetUp(Canvas mapCanvas)
    {
        MapCanvas = mapCanvas;
        View.scrollRect = mapCanvas.GetComponentInChildren<ScrollRect>(true);
        View.closeButton = mapCanvas.transform.GetChild(1).GetComponent<Button>();
    }
    
    public void GenerateMap()
    {
        //맵 생성
        CurrentMap = mapGenerator.GenerateMap(config);
        View.CreateMapView(CurrentMap);
    }

    public void ShowMap(ShowType type = ShowType.Select)
    {
        if (CurrentMap == null) return;
        
        //맵 캔버스 활성화 및 맵 뷰 생성
        MapCanvas.gameObject.SetActive(true);
        if (View.Map == null || View.Map != CurrentMap)
        {
            View.CreateMapView(CurrentMap);
        }
        
        //맵 뷰 셋팅
        View.SetShowType(type);
    }

    public void HideMap()
    {
        if(CurrentMap == null) return;

        MapCanvas.gameObject.SetActive(false);
    }
    
}
