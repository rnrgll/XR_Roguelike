using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerActor
{
    void StartTurn();
    bool IsTurnFinished();
    void RestoreHP(); // ← 추가
}
