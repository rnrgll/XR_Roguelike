using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    private bool turnEnded;

    private void Start()
    {
        Managers.Manager.Turn.RegisterPlayer(this);
    }

    public void StartTurn()
    {
        Debug.Log("플레이어 턴 시작!");
        turnEnded = false;

        // 에너지 회복 등 시작 준비
        // 카드 UI 활성화 등
    }

    public void EndTurn()
    {
        Debug.Log("플레이어 턴 종료!");
        turnEnded = true;

        // 카드 UI 비활성화 등
    }

    public bool IsTurnFinished()
    {
        return turnEnded;
    }
}
