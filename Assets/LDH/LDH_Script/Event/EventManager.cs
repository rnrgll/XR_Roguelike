using DesignPattern;
using DG.Tweening;
using Event;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    [SerializeField] private EventUI _model;
    [SerializeField] private RectTransform _contentRect; // 애니메이션 대상
    public GameEvent currentEvent;

    private void Awake() => SingletonInit();
    private void Start()
    {
        if (!Manager.Data.EventDB.IsReady)
        {
            Manager.Data.EventDB.OnDataLoadEnd += () =>
            {
                EventStart();
                AnimateUI();
            };
        }
        else
        {
            EventStart();
            AnimateUI();
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
        Release();
    }
    
    
    
    private void AnimateUI()
    {
        // 아래쪽에서 시작 → 중앙으로 이동
        Vector2 startPos = new Vector2(0, -Screen.height); // or sf.anchoredPosition - offset
        Vector2 endPos = Vector2.zero;

        _contentRect.anchoredPosition = startPos;
        _contentRect.DOAnchorPos(endPos, 0.8f)
            .SetEase(Ease.OutBack);
    }
    
}
