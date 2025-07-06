using System.Collections;
using System.Collections.Generic;
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

    // 현재 StatusUI가 비활성화되어있어 잠시 주석처리했습니다.
    public int ExecuteCombinationAttack(CardCombinationEnum combo, int nums, EnemyBase target, PlayerController pc)
    {
        int baseDamage = nums;
        var cc = pc.GetCardController();
        float multiplier =
            cc.ComboMultDic[combo] +
            cc.GetCardBonus(CardBonus.Mult);
        int bonus = pc.GetFlatAttackBonus() + (int)TurnManager.Instance.GetPlayerController().GetCardController().GetCardBonus(CardBonus.Score);
        float ratio = 1 + cc.GetCardBonus(CardBonus.Ratio);
        int finalDamage = Mathf.RoundToInt((baseDamage * multiplier + bonus) * ratio);

        // 기본 데미지 적용
        target.ApplyDamage(finalDamage);

        GameStatusUI.Instance.AddDamage(baseDamage);
        Debug.Log($"[{combo}] → {target.name}에게 {finalDamage}의 피해!");
        GameStatusUI.Instance.AddDamage(finalDamage);

        return finalDamage;
    }

}