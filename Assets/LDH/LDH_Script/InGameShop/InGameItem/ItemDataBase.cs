using CustomUtility.IO;
using DesignPattern;
using DG.Tweening.Plugins.Core.PathCore;
using Dialogue;
using Item;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using TopBarUI;
using Unity.VisualScripting;
using UnityEngine;
using Path = System.IO.Path;

namespace InGameShop
{
    public class ItemDataBase
    {
        public List<GameItem>
            ItemDB
        {
            get;
            private set;
        } //todo : 아이템 만들면 scriptable obj 를 불러오던지, csv 파일을 읽어서 아이템만들어서 넣어주던지 하는 방식으로 DB 데이터 셋팅하는 방식으로 변경필요

        public int InventoryItemCount { get; private set; } // 강화카드 제외 아이템 개수
        public int CardItemTotalCount { get; private set; } // 강화카드 아이템 개수
        public string itemSpriteFolder = "TestSprites"; //todo : 수정 필요
        public string cardSpriteFolder = "ArcanaTest"; //todo: 수정 필요



        public ItemDataBase()
        {
            //todo: itemDB 초기화 및 데이터 셋팅 수정 필요
            ItemDB = new();
            LoadItemData(ItemType.Item, "TestItemData.csv");
            LoadItemData(ItemType.Card, "TestCardItemData.csv");

        }

        public bool LoadItemData(ItemType type, string fileName, char splitSymol = ',')
        {
            #region Legacy

            // //<아이템 형>
            // //1. CSV 테이블 생성
            // CsvTable table = new CsvTable($"Data/Item/{fileName}");
            //
            // //2. Reader로 파일 읽기
            // CsvReader.Read(table);
            //
            // //3. 아이템 데이터 파싱
            // switch (type)
            // {
            //     case ItemType.Item:
            //         ParseItemData(table);
            //         break;
            //     case ItemType.Card:
            //         ParseCardItemData(table);
            //         break;
            // }
            //

            #endregion

            //인벤토리 아이템 로드
            InventoryItem[] inventoryItems = Resources.LoadAll<InventoryItem>("ScriptableObjects/GameItem");
            InventoryItemCount = inventoryItems.Length;
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                ItemDB.Add(inventoryItem);
            }

            //카드 아이템 로드
            CardItem[] cardItems = Resources.LoadAll<CardItem>("ScriptableObjects/GameItem");
            CardItemTotalCount = cardItems.Length;
            foreach (CardItem cardItem in cardItems)
            {
                ItemDB.Add(cardItem);
            }

            Debug.Log($"Item DB - 현재까지 불러온 아이템 수 : {ItemDB.Count}");

            //파싱 완료
            return true;
        }


        #region Item Test code

        public void ParseItemData(CsvTable table)
        {
            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);

            InventoryItemCount = rowCnt - 1; //아이템 개수 저장

            Dictionary<string, int> columnMap = new();
            for (int c = 0; c < columnCnt; c++)
                columnMap[table.Table[0, c]] = c;

            for (int r = 1; r < rowCnt; r++)
            {
                GameItem item = new InventoryItem()
                {
                    id = table.Table[r, columnMap["id"]],
                    name = table.Table[r, columnMap["name"]],
                    description = table.Table[r, columnMap["description"]],
                    price = int.Parse(table.Table[r, columnMap["price"]]),
                    sprite = Resources.Load<Sprite>(Path.Combine(itemSpriteFolder, table.Table[r, columnMap["image"]])),
                    weight = float.Parse(table.Table[r, columnMap["weight"]]),
                };
                ItemDB.Add(item);
            }

        }

        public void ParseCardItemData(CsvTable table)
        {
            int rowCnt = table.Table.GetLength(0);
            int columnCnt = table.Table.GetLength(1);

            CardItemTotalCount = rowCnt - 1; //아이템 개수 저장

            Dictionary<string, int> columnMap = new();
            for (int c = 0; c < columnCnt; c++)
                columnMap[table.Table[0, c]] = c;

            for (int r = 1; r < rowCnt; r++)
            {
                GameItem item = new CardItem()
                {
                    id = table.Table[r, columnMap["id"]],
                    name = table.Table[r, columnMap["name"]],
                    description = table.Table[r, columnMap["description"]],
                    price = int.Parse(table.Table[r, columnMap["price"]]),
                    sprite = Resources.Load<Sprite>(Path.Combine(cardSpriteFolder, table.Table[r, columnMap["image"]])),
                    weight = float.Parse(table.Table[r, columnMap["weight"]]),
                };
                ItemDB.Add(item);
            }
        }


        #endregion

        public int PickIndexRandomByWeight(List<GameItem> tempList)
        {
            float totalWeight = tempList.Sum(item => item.weight);
            float roll = Manager.randomManager.RandFloat(0, totalWeight);
            float current = 0;

            for (int i = 0; i < tempList.Count; i++)
            {
                current += tempList[i].weight;
                if (roll < current)
                    return i;
            }

            return tempList.Count - 1;
        }

        public List<string> PickUniqeItemRandomByType(int count, ItemType itemType = ItemType.Both)
        {
            List<GameItem> tempList = itemType switch
            {
                ItemType.Item => ItemDB.GetRange(0, InventoryItemCount),
                ItemType.Card => ItemDB.GetRange(InventoryItemCount, CardItemTotalCount),
                ItemType.Both => new List<GameItem>(ItemDB)
            };
            List<string> results = new();

            while (results.Count < count)
            {
                int pickedIndex = PickIndexRandomByWeight(tempList);
                results.Add(tempList[pickedIndex].id);

                int lastIdx = tempList.Count - 1;
                (tempList[pickedIndex], tempList[lastIdx]) = (tempList[lastIdx], tempList[pickedIndex]);

                tempList.RemoveAt(lastIdx);
            }

            return results;

        }



        public GameItem GetItemById(string id)
        {
            return ItemDB.FirstOrDefault(item => item.id == id);
        }
    }
}