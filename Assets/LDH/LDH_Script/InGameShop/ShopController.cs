using DesignPattern;
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
        public ShopTest _itemDatabase; //디버깅 용
        
        //생성자
        public ShopController(ShopModel model)
        {
            _model = model;
        }
        
        //아이템 드로우
        public void Reroll()
        {
            //가중치 기반으로 랜덤으로 아이템을 가져온다.
            var newItems = _itemDatabase.PickUniqeItemRandom(4);
            _model.SetItems(newItems);
            
        }

        public void Purchase(int itemID)
        {

            var item = _itemDatabase.GetItemById(itemID);
            
            if (GameStateManager.Instance.Gold < item.price)
                return;

            GameStateManager.Instance.AddGold(-item.price);
            //InventoryManager.Instance.AddItem(item);
            Debug.Log($"현재 보유 재화 : {GameStateManager.Instance.Gold}");
            
            Debug.Log($"인덴토리에 {item.id} - {item.name} 이 추가됩니다.");
            
            //popup 닫고, 비활성화 처리하기
            
        }
    }
}