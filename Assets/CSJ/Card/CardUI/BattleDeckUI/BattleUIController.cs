using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private UIRequire[] UIArr;
    // [SerializeField] private RootingUIController RootingScene;

    public void InitScene(PlayerController pc)
    {
        foreach (var ui in UIArr)
        {
            ui.InitializeUI(pc);
        }
    }

    public void PrintRootUI(MajorArcanaSO majorArcana)
    {
        Manager.UI.RootingUI.GetComponent<RootingUIController>().SetText(majorArcana);
        Manager.UI.SetUIActive(GlobalUI.Rooting, true);
    }

}
