using InGameShop;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : DesignPattern.Singleton<GameStateManager>
{
    public int Wins { get; private set; }
    public bool BossDefeated { get; private set; }
    public int ExternalCurrency { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }

    public readonly List<string> Inventory = new(); //아이템 id만 들고 있는다
    public int Item { get; private set; }

    public MonsterID SelectedMonster { get; set; }

    // 이벤트 추가
    public UnityEvent<int> OnGoldChanged = new();
    public UnityEvent<string> OnItemChanged = new();
    
    private void Awake()
    {
        SingletonInit();
    }

    
    //게임 시작 버튼 누르면 값 초기화 및 셋팅 (골드 보유량, 인벤토리 등등)
    public void Init()
    {
        for(int i=0; i<3; i++)
            Inventory.Add(String.Empty);
    }
    
    

    public void AddWin() => Wins++;
    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public void AddExp(int amount) => Exp += amount;

    public void AddItem(int amount) => Item += amount;

    public void AddItem(string itemID)
    {
        if (!Inventory.Contains(itemID))
        {
            int idx = Inventory.FindIndex(id => string.IsNullOrEmpty(id));
            Inventory[idx] = itemID;
            OnItemChanged?.Invoke(itemID);
        }
        
    }

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