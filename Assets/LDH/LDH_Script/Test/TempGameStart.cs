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
        public List<string> testItemIDList = new();
        public void GameStart()
        {
            Manager.Map.GenerateMap();
            Manager.UI.TopBarUI.SetActive(true);
            Manager.GameState.Init();
            Manager.GameState.AddGold(1000);
            
            // //랜덤으로 아이템 하나 획득 처리
            // List<string> items = Manager.Data.ItemDB.PickUniqeItemRandomByType(1, ItemType.Item);
            // Debug.Log(items[0]);
            // int randIdx = Manager.randomManager.RandInt(0, testItemIDList.Count);
            // string randItem = testItemIDList[randIdx];
            // Manager.GameState.AddItem(randItem);
            Manager.GameState.AddItem("207");
            Manager.GameState.AddItem("208");
            // _tarotDeck.AddMajorCards(_startCard);
        }

        
    }
}