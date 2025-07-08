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
        [SerializeField] private CardView _cardPrefab;
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
            Transform[] children = new Transform[_gridContainer.childCount];
            for (int i = 0; i < _gridContainer.childCount; i++)
                children[i] = _gridContainer.GetChild(i);
            
            foreach (Transform child in children)
            {
                if (child == null) continue; // 혹시라도 이미 삭제된 경우 방지
                
                CardView cardView = child.GetComponent<CardView>();
                cardView.DebugLog();
                child.GetComponent<CardView>().ResetData();
                
                var card = child.GetComponent<CardView>() as PooledObject;
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
                //Debug.Log($"{minor.CardSuit} {minor.CardNum} 카드 보여줄 차례");
                CardView card = _cardPool.PopPool() as CardView;
                card.transform.SetParent(_gridContainer);
                card.transform.SetSiblingIndex(idx++);
                card.SetData(minor);
            }


            //메이저 아르카나
            var majorArcanaList = new List<MajorArcanaSO>(_tarotDeck.GetMajorCards());
            if (majorArcanaList == null) return;
            foreach (var major in majorArcanaList)
            {
                CardView card = _cardPool.PopPool() as CardView;
                card.SetData(major);
                card.transform.SetParent(_gridContainer);
                card.transform.SetSiblingIndex(idx++);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_gridContainer as RectTransform);


        }


    }
}