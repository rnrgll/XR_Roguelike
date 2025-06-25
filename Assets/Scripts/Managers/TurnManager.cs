using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new List<IEnemyActor>();
    private bool battleStarted = false;

    private void Awake() => SingletonInit();

    public void RegisterPlayer(IPlayerActor p) => player = p;
    public void RegisterEnemy(IEnemyActor e) => enemies.Add(e);

    public void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (true)
        {
            // ArcanaManager.Instance.ApplyArcana();
            // DeckManager.Instance.DrawUntilHandLimit();
            //
            // player.StartTurn();
            // yield return new WaitUntil(() => player.IsTurnFinished());
            //
            // DeckManager.Instance.CleanHandAfterTurn();
            //
            // foreach (var enemy in enemies)
            // {
            //     if (enemy is MonoBehaviour mb && mb != null && (enemy as Enemy)?.IsDead == false)
            //     {
            //         enemy.TakeTurn();
            //         yield return new WaitForSeconds(0.5f);
            //     }
            // }
            //
            // ArcanaManager.Instance.PrepareNextArcana();
        }
    }
}