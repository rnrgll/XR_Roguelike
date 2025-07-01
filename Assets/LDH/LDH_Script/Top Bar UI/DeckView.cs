using DesignPattern;
using InGameShop;
using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopBarUI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private CardVeiw _cardPrefab;
        [SerializeField] private Transform _gridContainer;
        
        [SerializeField] private CardController _cardController;
        [SerializeField] private TarotDeck _tarotDeck;
        private ObjectPool _cardPool;

        private void Awake()
        {
            _cardPool = new(transform, _cardPrefab, 67);
        }

        private void OnEnable()
        {
            _cardController ??= FindObjectOfType<CardController>();
            _tarotDeck ??= FindObjectOfType<TarotDeck>();
            if(_cardController!=null && _tarotDeck != null)
                SettingCardView();
        }

        private void OnDisable()
        {
            foreach (Transform child in _gridContainer)
            {
                var card = child.GetComponent<CardVeiw>() as PooledObject;
                card.ReturnPool();
            }
        }
        private void SettingCardView()
        {
            //컨트롤러에서 전체 카드 덱을 조회한다.
            
            //마이너 아르카나
            if(_cardController.DeckPile==null) return;
            var minorArcanaList = _cardController.DeckPile.GetCardList();
            foreach (var minor in minorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(minor);
                card.transform.SetParent(_gridContainer);
                card.transform.SetAsLastSibling();
            }

            //메이저 아르카나
            var majorArcanaList = _tarotDeck.GetMajorCards();
            if (majorArcanaList == null) return;
            foreach (var major in majorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(major);
                card.transform.SetParent(_gridContainer);
                card.transform.SetAsLastSibling();
            }
            
            //카드 아이템
            foreach (string cardId in Manager.GameState.CardInventory)
            {
                var itemData = Manager.Data.ItemDB.GetItemById(cardId) as CardItem;
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(itemData);
                card.transform.SetParent(_gridContainer);
                card.transform.SetAsLastSibling();
            }
        }
        
        
    }
}