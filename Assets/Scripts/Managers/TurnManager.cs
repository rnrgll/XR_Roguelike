using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class TurnManager : Singleton<TurnManager>
    {
        private IPlayerActor player;
        private List<IEnemyActor> enemies = new List<IEnemyActor>();

        private void Awake() => SingletonInit();

        public void RegisterPlayer(IPlayerActor p) => player = p;
        public void RegisterEnemy(IEnemyActor e) => enemies.Add(e);

        public void StartBattle() => StartCoroutine(BattleLoop());

        private IEnumerator BattleLoop()
        {
            while (true)
            {
                // 1. 턴 시작: 아르카나 적용
                ArcanaManager.Instance.ApplyArcana();

                // 2. 핸드 보충
                DeckManager.Instance.DrawUntilHandLimit();

                // 3. 플레이어 턴
                player.StartTurn();
                yield return new WaitUntil(() => player.IsTurnFinished());

                // 4. 핸드 정리
                DeckManager.Instance.CleanHandAfterTurn();

                // 5. 적 턴
                foreach (var enemy in enemies)
                {
                    enemy.TakeTurn();
                    yield return new WaitForSeconds(0.5f);
                }

                ArcanaManager.Instance.PrepareNextArcana();
            }
        }
    }