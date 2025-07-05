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
    private void Start()
    {
        if (!Manager.Data.EventDB.IsReady)
        {
            Manager.Data.EventDB.OnDataLoadEnd += EventStart;
        }
        else
        {
            EventStart();
        }
        
    }

    private void EventStart()
    {
        currentEvent = Manager.Data.EventDB.GetRandomEvent();
        _model.UpdateUI(currentEvent);
        Debug.Log(currentEvent.EventImage);

        Manager.Data.EventDB.OnDataLoadEnd -= EventStart;
    }


    public void EventEnd()
    {
        Manager.Map.ShowMap();
    }
    
}
