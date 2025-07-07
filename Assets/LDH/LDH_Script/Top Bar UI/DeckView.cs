using DesignPattern;
using InGameShop;
using Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopBarUI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private CardVeiw _cardPrefab;
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private ScrollRect _scrollRect;


        [SerializeField] private CardController _cardController;
        [SerializeField] private TarotDeck _tarotDeck;
        private ObjectPool _cardPool;

        private void Awake()
        {
            _cardPool = new(transform, _cardPrefab, 100);
        }

        private void OnEnable()
        {
            _cardController ??= FindObjectOfType<CardController>();
            _tarotDeck ??= FindObjectOfType<TarotDeck>();
            if (_cardController != null && _tarotDeck != null)
                SettingCardView();
        }

        private void OnDisable()
        {
            foreach (Transform child in _gridContainer)
            {
                var card = child.GetComponent<CardVeiw>() as PooledObject;
                card.ReturnPool();
            }
            ResetScroll();
        }

        private void ResetScroll()
        {
            _scrollRect.normalizedPosition = new Vector2(0, 1);

        }

        private void SettingCardView()
        {
            //컨트롤러에서 전체 카드 덱을 조회한다.
            int idx = 0;
            //마이너 아르카나
            if (_cardController.Deck == null) return;
            var minorArcanaList = new List<MinorArcana>(_cardController.DeckPile.GetCardList());

            foreach (var minor in minorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.transform.SetParent(_gridContainer, false);
                card.transform.SetSiblingIndex(idx++);
                card.SetData(minor);
            }


            //메이저 아르카나
            var majorArcanaList = new List<MajorArcanaSO>(_tarotDeck.GetMajorCards());
            if (majorArcanaList == null) return;
            foreach (var major in majorArcanaList)
            {
                CardVeiw card = _cardPool.PopPool() as CardVeiw;
                card.SetData(major);
                card.transform.SetParent(_gridContainer);
                card.transform.SetSiblingIndex(idx++);
            }

        }


    }
}