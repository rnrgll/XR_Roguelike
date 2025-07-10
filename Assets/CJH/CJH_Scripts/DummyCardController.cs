using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCardController : MonoBehaviour
{
    public static DummyCardController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddPanelty(int amount)
    {
        Debug.Log($"[패널티 적용] 패널티 {amount}");
    }

}