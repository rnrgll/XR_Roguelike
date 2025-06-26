using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public int Wins { get; private set; }
    public bool BossDefeated { get; private set; }

    public int ExternalCurrency { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddWin()
    {
        Wins++;
    }

    public void SetBossDefeated()
    {
        BossDefeated = true;
    }

    public void CalculateReward()
    {
        ExternalCurrency = Wins * 100;
        if (BossDefeated) ExternalCurrency += 200;
    }

    public void ResetState()
    {
        Wins = 0;
        BossDefeated = false;
        ExternalCurrency = 0;
    }
}