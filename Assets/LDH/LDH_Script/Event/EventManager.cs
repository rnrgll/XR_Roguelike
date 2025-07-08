using DesignPattern;
using DG.Tweening;
using Event;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    [SerializeField] private EventUI _model;
    [SerializeField] private RectTransform _contentRect; // 애니메이션 대상
    public GameEvent currentEvent;

    private void Awake()
    {
        // 이미 인스턴스가 있으면 이건 중복이므로 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad 안 씀 — 씬 전용 오브젝트로 유지
    }

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
        Debug.Log($"{currentEvent.EventName} {currentEvent.EventID}");

        Manager.Data.EventDB.OnDataLoadEnd -= EventStart;
    }


    public void EventEnd()
    {
        Manager.Map.ShowMap();
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
