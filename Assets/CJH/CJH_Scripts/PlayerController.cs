using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerActor
{
    private bool turnEnded;

    private void Start()
    {
        //Managers.Manager.Turn.RegisterPlayer(this);
    }

    public void StartTurn()
    {
        Debug.Log("�÷��̾� �� ����!");
        turnEnded = false;

        // ������ ȸ�� �� ���� �غ�
        // ī�� UI Ȱ��ȭ ��
    }

    public void EndTurn()
    {
        Debug.Log("�÷��̾� �� ����!");
        turnEnded = true;

        // ī�� UI ��Ȱ��ȭ ��
    }

    public bool IsTurnFinished()
    {
        return turnEnded;
    }
}
