using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new List<IEnemyActor>();
    private Enemy currentEnemy;
    private bool battleStarted = false;

    private void Awake() => SingletonInit();

    public void RegisterPlayer(IPlayerActor p) => player = p;
    public void RegisterEnemy(IEnemyActor e)
    {
        enemies.Add(e);
        currentEnemy = e as Enemy;
    }

    public void StartBattle()
    {
        if (battleStarted) return;
        battleStarted = true;
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        PlayerController pc = player as PlayerController;

        while (true)
        {
            // 1. 플레이어 사망 시
            if (player != null && pc.IsDead)
            {
                Debug.Log("게임 오버!");
                //GameOverUI.Instance.Show();
                yield break;
            }

            // 2. 몬스터 사망 시 → 다음 스테이지
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("스테이지 클리어! 다음 스테이지로...");
                //BattleManager.Instance.GiveReward(); //  보상 지급
                pc.RestoreHP();
                yield return new WaitForSeconds(1f);

                yield break;
            }

            // 턴 시작
            //ArcanaManager.Instance.ApplyArcana();
            //DeckManager.Instance.DrawUntilHandLimit();

            player.StartTurn();
            yield return new WaitUntil(() => player.IsTurnFinished());

            //DeckManager.Instance.CleanHandAfterTurn();

            foreach (var enemy in enemies)
            {
                if (enemy is MonoBehaviour mb && mb != null && (enemy as Enemy)?.IsDead == false)
                {
                    enemy.TakeTurn();
                    yield return new WaitForSeconds(0.5f);
                }
            }

            //ArcanaManager.Instance.PrepareNextArcana();
        }
    }
}