using InGameShop;
using Item;
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
    public IReadOnlyList<string> ItemInventory => _itemInventory;
    public int CurrentItemCount => _itemInventory.Count(id => !string.IsNullOrEmpty(id));


    private const int maxItemInventorySize = 3;
    public int MaxItemInventorySize => maxItemInventorySize;

    #endregion

    private PlayerController playerController;
    public PlayerController Player => playerController;


    public MonsterID SelectedMonster { get; set; }

    // 이벤트 추가
    public UnityEvent<int> OnGoldChanged = new();
    public UnityEvent<string> OnItemChanged = new();
    public UnityEvent<EnchantItem> OnGetEnchantItem = new();

    private void Awake()
    {
        SingletonInit();
    }


    //게임 시작 버튼 누르면 값 초기화 및 셋팅 (골드 보유량, 인벤토리 등등)
    public void Init()
    {
        _itemInventory.Clear();

        for (int i = 0; i < MaxItemInventorySize; i++)
            _itemInventory.Add(String.Empty);
        
        
        //골드 //todo:골드 초기화 값 수정 필요
        AddGold(1000);
    }
    public void RegisterPlayerController(PlayerController pc)
    {
        playerController = pc;
        Debug.Log($"[GameStateManager] PlayerController 등록됨: {pc.name}");
    }


    public void AddWin() => Wins++;
    public void AddGold(int amount)
    {
        if (amount == 0) return;
        Gold = Mathf.Max(0, Gold + amount);
        OnGoldChanged?.Invoke(Gold);
        Debug.Log($"골드 {amount}를 {(amount >= 0 ? "획득" : "감소")}!");
    }

    public void AddExp(int amount) => Exp += amount;


    #region Inventory API

    public void AddItem(string itemID)
    {

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

    public void AddCardItem(EnchantItem enchantItem)
    {
        var deck = Player.GetCardController().Deck;
        CardDeck cardDeck = TurnManager.Instance.GetPlayerController().GetCardController().Deck;
        MinorArcana card = cardDeck.GetEnchantableCard().FirstOrDefault(card =>
            card.CardSuit == enchantItem.Suit && card.CardNum == enchantItem.CardNum);
        cardDeck.Enchant(card, enchantItem.enchantSo);

        OnGetEnchantItem?.Invoke(enchantItem);
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