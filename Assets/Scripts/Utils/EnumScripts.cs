using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardEnum
{
    public enum MinorSuit { Wands, Cups, Swords, Pentacles, wildCard = 30, statusEffect = 50, Special = 99 }

    public enum MajorPosition { Upright, Reversed }

    // TODO : 추후 기획에 맞춰 조정
    public enum CardEnchant { none, Bonus, Mult, Wild, Glass, Steel, Gold, Lucky }

    public enum CardDebuff { none, Charm, Corruption, Rust, Injury }
    public enum CardStatus { DeckList, BattleDeck, Hand, Graveyard }

    public enum CardBonus { Score, Mult, Ratio }
    public enum BonusType { Bonus, Penalty }


    public enum CardCombinationEnum
    {
        HighCard,
        OnePair,
        TwoPair,
        Triple,
        Straight,
        Flush,
        FullHouse,
        FourCard,
        StraightFlush,
        FiveCard,
        FiveJoker
    }
    public enum CardSortEnum
    {
        Number, Suit
    }
}

namespace Map
{
    public enum NodeType
    {
        NotAssgined,
        Battle, // 전투
        Shop, // 상점
        Event, // 이벤트
        Boss, // 보스
    }

    public enum NodeState
    {
        Locked,
        Visited,
        Attainable
    }

    public enum ShowType
    {
        Select,
        View,
    }
}

namespace InGameShop
{
    public enum ItemType
    {
        Item,
        Card,
        Both,
    }
    public enum SortOrder
    {
        Item = 3,
        PopUp = 5,

    }

    public enum ButtonState
    {
        Active,
        Deactive,
    }

}

namespace Item
{
    public enum ShopType
    {
        Lobby = 1,
        InGame = 2,
    }
    
    public enum InventoryItemType
    {
        Recovery,
        Enforce,
        Special
    }
    
    public enum EffectType
    {
        None,
        Heal,
        BuffAttack,
        HPReduce,
        DrawCard,
        DiscardHand,
        Invincible,
        GainJoker,
    }
}


namespace UI
{
    public enum GlobalUI
    {
        TopBar,
        Map,
        Deck,
        Item,
        Setting,
        ItemRemove,
    }

    //Global UI 랑 enum 값 맞춰주기
    public enum ToggleUI
    {
        Map = 1,
        Deck,
        Item,
        Setting,
    }
    
}



namespace Event
{
    public enum EffectType
    {
        Buff = 10,
        Debuff,
        CompoundEffect,
        NoEffect,
    }

    public enum SubEffectType
    {
        NoEffect= 0,
        AttackBoost = 20,
        ResourceGain,
        ObtainItem,
        ObtainEnhancedCard,
        ResourceLoss,
    }
    
}
