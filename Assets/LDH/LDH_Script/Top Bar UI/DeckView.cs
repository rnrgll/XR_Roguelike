using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopBarUI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private CardVeiw _cardPrefab;
        [SerializeField] private Transform _gridContainer;
        private CardController _cardController;
        
        private void OnEnable()
        {
            _cardController ??= FindObjectOfType<CardController>();
        }

        private void SettingCardView()
        {
            //컨트롤러에서 전체 카드 덱을 조회한다.
            //_cardController.DeckPile.get
        }
        
        public void SetCard()
        {
            CardVeiw card = Instantiate(_cardPrefab, _gridContainer);
            card.SetData();
        }
        
    }
}