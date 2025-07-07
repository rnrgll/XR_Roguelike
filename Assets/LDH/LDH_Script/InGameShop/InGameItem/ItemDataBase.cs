using CardEnum;
using CustomUtility.IO;
using Item;
using Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Path = System.IO.Path;

namespace InGameShop
{
    public class ItemDataBase
    {
        public List<GameItem> ItemDB { get; private set; }
        public Dictionary<CardEnchant, GameItem> EnchantDB { get; private set; }
        public int InventoryItemCount { get; private set; } // 강화카드 제외 아이템 개수
        // public int CardItemTotalCount { get; private set; } // 강화카드 아이템 개수

        private string itemDataFolder = "ScriptableObjects/GameItem";

        public ItemDataBase()
        {
            //todo: itemDB 초기화 및 데이터 셋팅 수정 필요
            ItemDB = new();
            EnchantDB = new();
            //LoadItemData(ItemType.Item, "TestItemData.csv");
            //LoadItemData(ItemType.Card, "TestCardItemData.csv");

            LoadItemData();
            LoadCardData();
        }

        #region 데이터 로드

        public void LoadItemData()
        {
            //인벤토리 아이템 로드
            InventoryItem[] inventoryItems = Resources.LoadAll<InventoryItem>(itemDataFolder);
            InventoryItemCount = inventoryItems.Length;
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                ItemDB.Add(inventoryItem);
            }

            //Debug.Log($"Item DB - 현재까지 불러온 아이템 수 : {ItemDB.Count}");
        }

        public void LoadCardData()
        {
            //카드 아이템 로드
            EnchantItem[] enchantItems = Resources.LoadAll<EnchantItem>(itemDataFolder);
            foreach (EnchantItem enchantItem in enchantItems)
            {
                EnchantDB.Add(enchantItem.enchantType, enchantItem);
            }

            //Debug.Log($"Enchant DB - 로드 완료");
        }

        #endregion


        public GameItem GetItemById(string id)
        {
            return ItemDB.FirstOrDefault(item => item.id == id);
        }


        #region 아이템 랜덤 선택

        /// <summary>
        /// 아이템 타입애 해당하는 서로 다른 count개의 아이템을 뽑아서 아이템 아이디 리스트를 반환합니다.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public List<GameItem> PickUniqeItemRandomByType(int count, ItemType itemType = ItemType.Both)
        {
            
            if (itemType == ItemType.Item)
            {
                return PickRandomInventoryItems(count);
            }
            if (itemType == ItemType.Card)
            {
                return PickRandomEnchantItems(count);
            }
            else
            {
                int random = Manager.randomManager.RandInt(0, count);
                int enchantCount = TurnManager.Instance.GetPlayerController().GetCardController().Deck
                    .GetEnchantableCard().Count;
                
                enchantCount = Mathf.Min(enchantCount, random);
                List<GameItem> result = PickRandomEnchantItems(enchantCount);
                result.AddRange(PickRandomInventoryItems(count - enchantCount));
                return result;
            }
        }

        private int PickIndexRandomByWeight(List<GameItem> tempList)
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

        private List<GameItem> PickRandomEnchantItems(int count)
        {
            List<GameItem> results = new();
            
            var deck = TurnManager.Instance.GetPlayerController().GetCardController().Deck;
            var enchantables = deck.GetEnchantableCard();

            // 1. 카드가 부족하면 count 조정
            count = Mathf.Min(count, enchantables.Count);
            // 2. 셔플
            Shuffle(enchantables); 

            for (int i = 0; results.Count<count; i++)
            {
                var card = enchantables[i];
                
                
                // 3. 랜덤한 EnchantType 선택 (1~7 사이)
                int random = UnityEngine.Random.Range(1, 8);
                var enchantType = (CardEnchant)random;
                
                //4. enchant type의 target 범위에 들어가는지 체크
                EnchantItem origin = EnchantDB[enchantType] as EnchantItem;
                if(enchantables[i].CardNum > origin.maxTargetNum || enchantables[i].CardNum < origin.minTargetNum) continue; //타겟 범위 아님

                
                //타겟 범위인 경우
                EnchantItem enchantItem = ScriptableObject.Instantiate(origin);
                Debug.Log($"{enchantItem.enchantSo}, {card.CardSuit}, {card.CardNum}");
                enchantItem.SetData(card.CardSuit, card.CardNum);


                string cNum = card.CardNum.ToString();
                string cName = $"MinorArcana/{card.CardSuit}/{card.CardSuit}_{cNum}";
                var sprite = Resources.Load<Sprite>(cName);
                enchantItem.sprite = sprite;
                
                results.Add(enchantItem);
            }


            
            
            return results;
        }

        public List<GameItem> PickRandomPotionItems(int count)
        {
            List<GameItem> candidates = ItemDB
                .OfType<InventoryItem>()          // InventoryItem으로 필터링
                .Where(i => i.isPotion == true)   // 조건 필터
                .Cast<GameItem>()                 // GameItem으로 캐스팅 (필요한 경우)
                .ToList();        
            List<GameItem> results = new();
            while (results.Count < count)
            {
                int pickedIndex = PickIndexRandomByWeight(candidates);
                results.Add(candidates[pickedIndex]);

                int lastIdx = candidates.Count - 1;
                (candidates[pickedIndex], candidates[lastIdx]) = (candidates[lastIdx], candidates[pickedIndex]);

                candidates.RemoveAt(lastIdx);
            }

            return results;
            
        }
        
        private List<GameItem> PickRandomInventoryItems(int count)
        {
            List<GameItem> candidates = new(ItemDB);
            List<GameItem> results = new();
            while (results.Count < count)
            {
                int pickedIndex = PickIndexRandomByWeight(candidates);
                results.Add(candidates[pickedIndex]);

                int lastIdx = candidates.Count - 1;
                (candidates[pickedIndex], candidates[lastIdx]) = (candidates[lastIdx], candidates[pickedIndex]);

                candidates.RemoveAt(lastIdx);
            }

            return results;
        }

        
        public void Shuffle(List<MinorArcana> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rnd = Manager.randomManager.RandInt(i, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
        #endregion


        #region CSV Load(사용x)

        // public bool LoadItemData(ItemType type, string fileName, char splitSymol = ',')
        // {
        //
        //     // //<아이템 형>
        //     // //1. CSV 테이블 생성
        //     // CsvTable table = new CsvTable($"Data/Item/{fileName}");
        //     //
        //     // //2. Reader로 파일 읽기
        //     // CsvReader.Read(table);
        //     //
        //     // //3. 아이템 데이터 파싱
        //     // switch (type)
        //     // {
        //     //     case ItemType.Item:
        //     //         ParseItemData(table);
        //     //         break;
        //     //     case ItemType.Card:
        //     //         ParseCardItemData(table);
        //     //         break;
        //     // }
        //     //
        //
        //     return true;
        // }
        //

        // public void ParseItemData(CsvTable table)
        // {
        //     int rowCnt = table.Table.GetLength(0);
        //     int columnCnt = table.Table.GetLength(1);
        //
        //     InventoryItemCount = rowCnt - 1; //아이템 개수 저장
        //
        //     Dictionary<string, int> columnMap = new();
        //     for (int c = 0; c < columnCnt; c++)
        //         columnMap[table.Table[0, c]] = c;
        //
        //     for (int r = 1; r < rowCnt; r++)
        //     {
        //         GameItem item = new InventoryItem()
        //         {
        //             id = table.Table[r, columnMap["id"]],
        //             itemName = table.Table[r, columnMap["name"]],
        //             description = table.Table[r, columnMap["description"]],
        //             price = int.Parse(table.Table[r, columnMap["price"]]),
        //             sprite = Resources.Load<Sprite>(Path.Combine(itemSpriteFolder, table.Table[r, columnMap["image"]])),
        //             weight = float.Parse(table.Table[r, columnMap["weight"]]),
        //         };
        //         ItemDB.Add(item);
        //     }
        // }
        //
        // public void ParseCardItemData(CsvTable table)
        // {
        //     int rowCnt = table.Table.GetLength(0);
        //     int columnCnt = table.Table.GetLength(1);
        //
        //     //CardItemTotalCount = rowCnt - 1; //아이템 개수 저장
        //
        //     Dictionary<string, int> columnMap = new();
        //     for (int c = 0; c < columnCnt; c++)
        //         columnMap[table.Table[0, c]] = c;
        //
        //     for (int r = 1; r < rowCnt; r++)
        //     {
        //         GameItem item = new EnchantItem()
        //         {
        //             id = table.Table[r, columnMap["id"]],
        //             itemName = table.Table[r, columnMap["name"]],
        //             description = table.Table[r, columnMap["description"]],
        //             price = int.Parse(table.Table[r, columnMap["price"]]),
        //             sprite = Resources.Load<Sprite>(Path.Combine(cardSpriteFolder, table.Table[r, columnMap["image"]])),
        //             weight = float.Parse(table.Table[r, columnMap["weight"]]),
        //         };
        //         ItemDB.Add(item);
        //     }
        // }

        #endregion
    }
}