using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardEnum;
using Unity.Burst.Intrinsics;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int ExecuteCombinationAttack(CardCombinationEnum combo, List<int> nums, EnemyBase target)
    {
        int baseDamage = nums.Sum();
        float multiplier = TurnManager.Instance.GetPlayerController().GetAttackMultiplier();
        int bonus = TurnManager.Instance.GetPlayerController().GetFlatAttackBonus();
        int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);


        GameStatusUI.Instance.SetComboInfo(combo.ToString(), multiplier);


        // 기본 데미지 적용
        target.ApplyDamage(finalDamage);

        GameStatusUI.Instance.AddDamage(baseDamage);
        Debug.Log($"[{combo}] → {target.name}에게 {baseDamage}의 피해!");

        return finalDamage;
    }

}