using DesignPattern;
using Event;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    [SerializeField] private EventUI _model;
    public GameEvent currentEvent;
    
    private void Awake() => SingletonInit();
    
    public void GameStart()
    {
        currentEvent = Manager.Data.EventDB.GetGameEventById(1);
        _model.UpdateUI(currentEvent);
    }
    
    
}
