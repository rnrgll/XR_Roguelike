using InGameShop;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UI;
using Unity.Mathematics;

public class GameStateManager : DesignPattern.Singleton<GameStateManager>
{
    public int Wins { get; private set; }
    public bool BossDefeated { get; private set; }
    public int ExternalCurrency { get; private set; }
    public int Gold { get; private set; }
    public int Exp { get; private set; }

    #region Inventory 변수, 프로퍼티

    private readonly List<string> _itemInventory = new();
    private readonly List<string> _cardInventory = new(); // 카드용
    public IReadOnlyList<string> ItemInventory => _itemInventory;
    public IReadOnlyList<string> CardInventory => _cardInventory;
    public int CurrentItemCount => _itemInventory.Count(id => !string.IsNullOrEmpty(id));
    public int CurrentCardItemCount => _itemInventory.Count;


    private const int maxItemInventorySize = 3;
    public int MaxItemInventorySize => maxItemInventorySize;

    #endregion




    public MonsterID SelectedMonster { get; set; }

    // 이벤트 추가
    public UnityEvent<int> OnGoldChanged = new();
    public UnityEvent<string> OnItemChanged = new();
    public UnityEvent<string> OnCardItemChanged = new();

    private void Awake()
    {
        SingletonInit();
    }


    //게임 시작 버튼 누르면 값 초기화 및 셋팅 (골드 보유량, 인벤토리 등등)
    public void Init()
    {
        _itemInventory.Clear();
        _cardInventory.Clear();

        for (int i = 0; i < MaxItemInventorySize; i++)
            _itemInventory.Add(String.Empty);
    }



    public void AddWin() => Wins++;
    public void AddGold(int amount)
    {
        if(amount==0) return;
        Gold = Mathf.Max(0, Gold + amount);
        OnGoldChanged?.Invoke(Gold);
        Debug.Log($"골드 {amount}를 {(amount>=0?"획득":"감소")}!");
    }

    public void AddExp(int amount) => Exp += amount;


    #region Inventory API

    public void AddItem(string itemID)
    {
        //인벤토리 슬롯이 꽉 차있는지 확인
        if (CurrentItemCount == 3)
        {
            //아이템 제거하도록 처리
            Manager.UI.ItemRemoveUI.SetCallBack(() => AddItem(itemID));
            Manager.UI.SetUIActive(GlobalUI.ItemRemove, true);
            return;
        }
        
        int idx = _itemInventory.FindIndex(id => string.IsNullOrEmpty(id));
        if (idx != -1)
        {
            _itemInventory[idx] = itemID;
            OnItemChanged?.Invoke(itemID);
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다. 아이템 제거가 제대로 이루어지지 않았습니다.");
            // 또는 UI 경고 처리
        }
    }

    public void AddCardItem(string cardId)
    {
        Debug.Log(cardId);
        _cardInventory.Add(cardId);
        OnCardItemChanged?.Invoke(cardId);
    }
    public void RemoveItem(string itemID)
    {
        int idx = _itemInventory.FindIndex(id => id == itemID);
        if (idx != -1)
        {
            _itemInventory[idx] = string.Empty;
            OnItemChanged?.Invoke(itemID);
        }
    }

    public void RemoveCardItem(string cardId)
    {
        int idx = _cardInventory.FindIndex(id => id == cardId);
        _cardInventory.RemoveAt(idx);
        OnCardItemChanged?.Invoke(cardId);
    }

    #endregion

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