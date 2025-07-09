using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private UIRequire[] UIArr;
    [SerializeField] private RootingUIController RootingScene;

    public void InitScene(PlayerController pc)
    {
        foreach (var ui in UIArr)
        {
            ui.InitializeUI(pc);
        }
    }

    public void PrintRootUI(MajorArcanaSO majorArcana)
    {
        RootingScene.SetActive(true);
        RootingScene.SetText(majorArcana);
    }

}
