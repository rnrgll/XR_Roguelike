using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    private bool isDead = false;
    public bool IsDead => isDead;

    private void Start()
    {
        currentHP = maxHP;
        GameStatusUI.Instance.Init(maxHP);
    }

    public void ApplyDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} 피해: {damage}, 남은 체력: {currentHP}");

        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} 사망!");
            Destroy(gameObject);
        }
    }

}