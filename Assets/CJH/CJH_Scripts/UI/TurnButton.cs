using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnButton : MonoBehaviour
{
    public void OnClickEndTurn()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player.EndTurn();
    }
}