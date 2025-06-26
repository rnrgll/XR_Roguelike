using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward
{
    public int gold;
    public int experience;
    public string rewardText => $"Game Clear!\n\n" +
                                $"제거한 죄악의 수 : {GameStateManager.Instance.Wins} (보스 포함)\n" +
                                $"획득 재화 : {GameStateManager.Instance.ExternalCurrency}";
}