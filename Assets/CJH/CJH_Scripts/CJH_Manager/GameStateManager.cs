using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : Singleton<GameStateManager>
{
    public int Wins { get; private set; }
    public bool BossDefeated { get; private set; }
    public int ExternalCurrency { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }
    public int Item { get; private set; }
    
    
    // 이벤트 추가
    public UnityEvent<int> OnGetGold = new();

    private void Awake()
    {
        SingletonInit();
        Debug.Log($"{GetType().Name} singleton 초기화 완료");
    }

    public void AddWin() => Wins++;
    public void AddGold(int amount)
    {
        Gold += amount;
        OnGetGold?.Invoke(Gold);
    }

    public void AddExp(int amount) => Exp += amount;

    public void AddItem(int amount) => Item += amount;

    public void SetBossDefeated()
    {
        BossDefeated = true;
    }

    public void CalculateReward()
    {
        ExternalCurrency = Wins * 100;
        if (BossDefeated) ExternalCurrency += 200;
    }

    public void ResetState()
    {
        Wins = 0;
        BossDefeated = false;
        ExternalCurrency = 0;
    }
}