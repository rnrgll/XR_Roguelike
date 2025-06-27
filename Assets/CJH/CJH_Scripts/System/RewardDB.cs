using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDB : MonoBehaviour
{
    public static RewardDB Instance;

    public List<ItemReward> itemPool;
    public int fixedGoldPerWin = 50;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<ItemReward> GetRandomItemRewards(int count)
    {
        List<ItemReward> selected = new();
        var pool = new List<ItemReward>(itemPool);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int rand = Random.Range(0, pool.Count);
            selected.Add(pool[rand]);
            pool.RemoveAt(rand);
        }

        return selected;
    }

    public int GetFixedGold() => fixedGoldPerWin;
}