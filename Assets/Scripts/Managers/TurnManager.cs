using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TurnManager : Singleton<TurnManager>
{
    private IPlayerActor player;
    private List<IEnemyActor> enemies = new List<IEnemyActor>();
    private Enemy currentEnemy;
    private bool battleStarted = false;
    private bool isGameEnded = false;

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
        if (player == null)
        {
            Debug.LogError("⚠️ [TurnManager] player가 null이다! 피의 맹세가 깨졌도다!");
            yield break;
        }

        if (pc == null)
        {
            Debug.LogError("⚠️ [TurnManager] player가 PlayerController로 캐스팅되지 않았노라! 흠… 흠흠, 부끄럽도다!");
            yield break;
        }

        if (pc.IsDead)
        {
            Debug.Log("게임 오버!");

            if (GameOverUI.Instance == null)
            {
                Debug.LogError("⚠️ GameOverUI가 존재하지 않는다! 전장에 UI가 빠졌도다!");
                yield break;
            }

            GameOverUI.Instance.Show();
            isGameEnded = true;
            yield break;
        }

        while (true)
        {
            // 1. 플레이어 사망 시
            if (player != null && pc.IsDead)
            {
                Debug.Log("게임 오버!");
                GameOverUI.Instance.Show();
                isGameEnded = true;
                yield break;
            }

            // 2. 몬스터 사망 시 → 다음 스테이지
            if (currentEnemy == null || currentEnemy.IsDead)
            {
                Debug.Log("전투 승리!");
                GameStateManager.Instance.AddWin();

                if (currentEnemy != null && currentEnemy.IsBoss)
                {
                    GameStateManager.Instance.SetBossDefeated();
                    GameClearUI.Instance.Show();
                    isGameEnded = true;
                }
                else
                {
                    pc.RestoreHP();
                    // SceneManager.LoadScene("");
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
}