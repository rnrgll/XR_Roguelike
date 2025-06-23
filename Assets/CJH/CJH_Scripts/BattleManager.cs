using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExecuteAttack(List<Card> cards, Enemy target)
    {
        int powerSum = 0;
        float multiplier = 1.0f;

        foreach (var card in cards)
        {
            powerSum += card.power;
            multiplier *= card.GetAttributeMultiplier(target); // 속성 상성
        }

        int finalDamage = Mathf.FloorToInt(powerSum * multiplier);
        target.ApplyDamage(finalDamage);
    }
}