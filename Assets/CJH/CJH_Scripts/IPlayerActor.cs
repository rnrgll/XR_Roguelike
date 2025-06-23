using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerActor
{
    void StartTurn();
    bool IsTurnFinished(); // 턴 종료 조건 체크
}