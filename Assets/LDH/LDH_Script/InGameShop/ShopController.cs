using DesignPattern;
using Item;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace InGameShop
{
    public class ShopController
    {
        private ShopModel _model;
        //생성자
        public ShopController(ShopModel model)
        {
            _model = model;
        }
        
        //아이템 드로우
        public void Reroll()
        {
            //가중치 기반으로 랜덤으로 아이템을 가져온다.
            var newItems = Manager.Data.GameItemDB.PickUniqeItemRandomByType(4);
            _model.SetItems(newItems);
            
        }

        public void Purchase(GameItem item)
        {
            if (GameStateManager.Instance.Gold < item.price)
                return;

            GameStateManager.Instance.AddGold(-item.price);

            if (item is InventoryItem)
            {
                Manager.GameState.AddItem(item.id);
                Debug.Log($"인덴토리에 {item.id} - {item.itemName} 이 추가됩니다.");
            }

            else
            {
                
                Manager.GameState.AddCardItem(item as EnchantItem);
                EnchantItem enchantItem = item as EnchantItem;
                Debug.Log($"덱에 {enchantItem.enchantType} - {item.itemName} {enchantItem.Suit} {enchantItem.CardNum} 이 추가됩니다.");
            }
            
            Debug.Log($"현재 보유 재화 : {GameStateManager.Instance.Gold}");
        
            //popup 닫고, 비활성화 처리하기
            
        }
    }
}