using InGameShop;
using Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TempGameStart : MonoBehaviour
    {
        [SerializeField] private TarotDeck _tarotDeck;
        [SerializeField] private MajorArcanaSO _startCard;
        public void GameStart()
        {
            Manager.Map.GenerateMap();
            Manager.UI.TopBarUI.SetActive(true);
            Manager.GameState.Init();
            Manager.GameState.AddGold(1000);
            
            // //랜덤으로 아이템 하나 획득 처리
            List<string> items = Manager.Data.ItemDB.PickUniqeItemRandomByType(1, ItemType.item);
            Manager.GameState.AddItem(items[0]);
            
            
            _tarotDeck.AddMajorCards(_startCard);
        }

        
    }
}