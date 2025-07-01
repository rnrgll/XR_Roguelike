using DesignPattern;
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
            if(_cardController.DeckPile==null) return;
            var minorArcanaList = _cardController.DeckPile.GetCardList();
            foreach (var minor in minorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(minor);
                card.transform.SetParent(_gridContainer);
                card.transform.SetAsLastSibling();
            }

            var majorArcanaList = _tarotDeck.GetMajorCards();
            if (majorArcanaList == null) return;
            foreach (var major in majorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(major);
                card.transform.SetParent(_gridContainer);
                card.transform.SetAsLastSibling();
            }
        }
        
        
    }
}