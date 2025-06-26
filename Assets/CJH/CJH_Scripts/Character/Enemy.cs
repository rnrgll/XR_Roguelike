using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    [SerializeField] private bool isBoss = false;
    private int currentHP;
    public bool IsDead { get; private set; }
    public bool IsBoss => isBoss;

    private void Start()
    {
        currentHP = maxHP;
        GameStatusUI.Instance.Init(maxHP);
    }

    public void ApplyDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} 피해: {damage}, 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log($"{gameObject.name} 사망!");
            IsDead = true;
            Destroy(gameObject);
        }


    }



}