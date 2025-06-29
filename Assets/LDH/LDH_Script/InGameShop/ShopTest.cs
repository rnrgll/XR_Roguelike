using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InGameShop
{
    [Serializable]
    public class TempItemClass
    {
        public int id;
        public string name;
        public string description;
        public int price;
        public Sprite image;
        public float weight;
    }
    public class ShopTest : MonoBehaviour
    {
        public List<TempItemClass> itemDB;
        
        private void Start()
        {
            //todo:testcode
            GameStateManager.Instance.AddGold(500);
        }

        public int PickIndexRandomByWeight(List<TempItemClass> tempList)
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

        public List<int> PickUniqeItemRandom(int count)
        {
            List<TempItemClass> tempList = new List<TempItemClass>(itemDB);
            
            List<int> results = new();

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

        public TempItemClass GetItemById(int id)
        {
           return itemDB.FirstOrDefault(item => item.id == id);
        }
        
    }
}