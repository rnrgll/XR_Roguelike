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

    public int eventID;
    private void Awake() => SingletonInit();
    
    public void GameStart()
    {
        currentEvent = Manager.Data.EventDB.GetGameEventById(eventID);
        _model.UpdateUI(currentEvent);
    }
    
    
}
