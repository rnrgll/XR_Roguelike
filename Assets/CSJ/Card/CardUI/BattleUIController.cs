using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private UIRequire[] UIArr;

    public void InitScene(PlayerController pc)
    {
        foreach (var ui in UIArr)
        {
            ui.InitializeUI(pc);
        }
    }
}
