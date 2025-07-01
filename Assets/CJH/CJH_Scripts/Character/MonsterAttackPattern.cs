using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterAttackPattern
{
    public string name;
    public MonsterAttackType type;

    [Range(0f, 1f)]
    public float damageRate = 0.1f;

    public string description;

    public int triggerTurn = 0; // 0이면 무시
    [Range(0f, 1f)]
    public float triggerHpRate = 0f; // 0이면 무시
}